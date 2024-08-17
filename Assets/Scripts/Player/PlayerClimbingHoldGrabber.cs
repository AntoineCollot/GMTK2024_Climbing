using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerClimbingHoldGrabber : MonoBehaviour
{
    InputMap inputs;

    [Header("Grab")]
    [SerializeField] Transform grabOrigin;
    [SerializeField] float grabRange;
    [SerializeField] LayerMask grabLayerMask;
    ClimbimgHold inReachHold;
    ClimbimgHold grabbedHold;

    [Header("Move")]
    [SerializeField] float slideToHoldTime;

    public UnityEvent onGrabHold = new UnityEvent();
    public UnityEvent onReleaseHold = new UnityEvent();

    public bool isGrabbingHold => grabbedHold != null;
    public bool hasHoldInReach => inReachHold != null;

    // Start is called before the first frame update
    void Start()
    {
        inputs = new InputMap();
        inputs.Enable();
        inputs.Gameplay.Catch.performed += CatchPerformed;
    }

    private void OnDestroy()
    {
        inputs.Gameplay.Jump.performed -= CatchPerformed;
        inputs.Disable();
        inputs.Dispose();
    }

    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(grabOrigin.position, grabRange, grabLayerMask);
        inReachHold = null;

        //Find the closest hold
        float minDist = Mathf.Infinity;
        foreach (Collider collider in colliders)
        {
            if(collider.TryGetComponent(out ClimbimgHold hold))
            {
                float dist = hold.DistanceFrom(grabOrigin.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    inReachHold = hold;
                }
            }
        }
    }

    private void CatchPerformed(InputAction.CallbackContext context)
    {
        if (isGrabbingHold)
            return;

        if(hasHoldInReach)
        {
            GrabHold(inReachHold);
        }
    }

    void GrabHold(ClimbimgHold hold)
    {
        if (hold == grabbedHold || hold == null)
            return;

        grabbedHold = hold;
        StartCoroutine(MoveToHold(hold));

        onGrabHold.Invoke();
    }

    IEnumerator MoveToHold(ClimbimgHold hold)
    {
        float t = 0;
        Vector3 offset = transform.position - grabOrigin.position;
        Vector3 startPosition = grabOrigin.position;
        Vector3 holdPosition = hold.GetGrabPosition(startPosition);
        while (t<1 && isGrabbingHold)
        {
            t += Time.deltaTime / slideToHoldTime;

            transform.position = Curves.QuadEaseInOut(startPosition, holdPosition, Mathf.Clamp01(t))+ offset;
            yield return null;
        }
    }

    public void ReleaseHold()
    {
        StopAllCoroutines();
        grabbedHold = null;
        onReleaseHold.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        if (hasHoldInReach)
            Gizmos.color = Color.blue;
        else
            Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(grabOrigin.position, grabRange);
    }
}
