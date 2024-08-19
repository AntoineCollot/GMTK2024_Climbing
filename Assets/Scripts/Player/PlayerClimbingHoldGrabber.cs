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
    [SerializeField] Transform grabDetectionOrigin;
    const float GRAB_Y_OFFSET = 0.075f;
    [SerializeField] float grabRange;
    [SerializeField] LayerMask grabLayerMask;
    ClimbingHold hoveredHold;
    ClimbingHold grabbedHold;
    GrabGizmos grabGizmos;

    [Header("Disable Grab")]
    [SerializeField] Material playerOutlineMat;
    float disabledGrabRemainingTime;
    const float MAX_DISABLED_TIME = 0.4f;
    static readonly Color GRAB_DISABLED_COLOR = new Color(0.4339623f, 0.03479887f, 0.03479887f, 1);
    const float OUTLINE_WIDTH = 0.04f;

    [Header("Move")]
    [SerializeField] float slideToHoldTime;

    public class HoldEvent : UnityEvent<ClimbingHold> { }
    public HoldEvent onGrabHold = new HoldEvent();
    public HoldEvent onReleaseHold = new HoldEvent();

    public bool isGrabbingHold => grabbedHold != null;
    public bool hasHoldInReach => hoveredHold != null;

    public Vector3 GrabPosition => transform.position + Vector3.up * GRAB_Y_OFFSET;
    public bool CanGrab => disabledGrabRemainingTime <= 0;

    // Start is called before the first frame update
    void Start()
    {
        grabGizmos = FindObjectOfType<GrabGizmos>(true);
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
        Collider[] colliders = Physics.OverlapSphere(grabDetectionOrigin.position, grabRange, grabLayerMask);
        ClimbingHold newHoveredHold = null;

        //Find the closest hold
        float minDist = Mathf.Infinity;
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out ClimbingHold hold))
            {
                float dist = hold.DistanceFrom(GrabPosition);
                if (dist < minDist)
                {
                    minDist = dist;
                    newHoveredHold = hold;
                }
            }
        }

        if (newHoveredHold != hoveredHold)
        {
            if (hoveredHold != null)
                hoveredHold.OnHoverExit();
            hoveredHold = newHoveredHold;
            if (hoveredHold != null)
                hoveredHold.OnHoverEnter();
        }

        disabledGrabRemainingTime -= Time.deltaTime;
        playerOutlineMat.SetFloat("_OutlineWidth", Curves.QuartEaseIn(OUTLINE_WIDTH, 0, 1-Mathf.Clamp01(disabledGrabRemainingTime / MAX_DISABLED_TIME)));
    }

    private void CatchPerformed(InputAction.CallbackContext context)
    {
        if (isGrabbingHold || !CanGrab)
            return;

        if (hasHoldInReach)
        {
            GrabHold(hoveredHold);
        }
        else
            FailGrab();
    }

    void GrabHold(ClimbingHold hold)
    {
        if (hold == grabbedHold || hold == null)
            return;

        grabbedHold = hold;
        grabbedHold.OnGrabbed(GrabPosition);
        StartCoroutine(MoveToHold(hold));

        onGrabHold.Invoke(hold);
    }

    void FailGrab()
    {
        grabGizmos.DisplayMissedGrab(grabDetectionOrigin.position, grabRange);
        DisableGrab();
    }

    void DisableGrab()
    {
        disabledGrabRemainingTime = MAX_DISABLED_TIME;
        playerOutlineMat.SetColor("_OutlineColor", GRAB_DISABLED_COLOR);
    }

    IEnumerator MoveToHold(ClimbingHold hold)
    {
        float t = 0;
        Vector3 startPosition = GrabPosition;
        Vector3 offset = transform.position - startPosition;
        Vector3 holdPosition = hold.GetGrabPosition(startPosition);
        while (t < 1 && isGrabbingHold)
        {
            t += Time.deltaTime / slideToHoldTime;

            transform.position = Curves.QuadEaseInOut(startPosition, holdPosition, Mathf.Clamp01(t)) + offset;
            yield return null;
        }
    }

    public void ReleaseHold()
    {
        StopAllCoroutines();
        ClimbingHold hold = grabbedHold;
        grabbedHold = null;
        if (hold != null)
        {
            hold.OnReleased();
            onReleaseHold.Invoke(hold);
        }
        DisableGrab();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (hasHoldInReach)
            Gizmos.color = Color.blue;
        else
            Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(grabDetectionOrigin.position, grabRange);
    }
#endif
}
