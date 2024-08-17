using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerJumpController : MonoBehaviour
{
    [Header("Jump")]
    [SerializeField] float minJumpForce;
    [SerializeField] float maxJumpForce;

    [SerializeField, Range(0, 1)] float smallJumpFactor = 0.3f;
    [SerializeField, Range(0.5f, 5)] float jumpHeight = 0.5f;
    [SerializeField, Range(0.2f, 2)] float timeToJumpApex = 0.5f;

    [SerializeField] float fullChargeTime;
    [SerializeField] float minChargeTime;
    float jumpCharge01;
    float preJumpTime01;
    InputMap inputs;

    bool isChargingJump;
    bool hasPreCharged;

    [Header("Fall")]
    [SerializeField] float gravityScale = 3.5f;
    float gravityMult = 1;
    float gravity;
    [SerializeField] float fallingGravityMultiplier = 1;
    [SerializeField] float maxFallSpeed;
    [SerializeField, Range(0, 1f)] float drag;

    PlayerClimbingHoldGrabber holdGrabber;

    Rigidbody body;
    Camera cam;

    public UnityEvent onJump = new UnityEvent();

    public bool IsFalling => body.velocity.y < -0.01f;
    public float JumpForce => Mathf.Sqrt(-2f * gravity * jumpHeight) * Mathf.Lerp(smallJumpFactor, 1, jumpCharge01);

    // Start is called before the first frame update
    void Start()
    {
        holdGrabber = GetComponentInChildren<PlayerClimbingHoldGrabber>();
        holdGrabber.onGrabHold.AddListener(OnGrabHold);
        body = GetComponent<Rigidbody>();
        cam = Camera.main;
        inputs = new InputMap();
        inputs.Enable();
        inputs.Gameplay.Jump.performed += JumpPerformed;
        inputs.Gameplay.Jump.canceled += JumpReleased;
    }

    private void OnDestroy()
    {
        if (holdGrabber != null)
            holdGrabber.onGrabHold.RemoveListener(OnGrabHold);
        inputs.Gameplay.Jump.performed -= JumpPerformed;
        inputs.Gameplay.Jump.canceled -= JumpReleased;
        inputs.Disable();
        inputs.Dispose();
    }

    private void FixedUpdate()
    {
        if (isChargingJump || holdGrabber.isGrabbingHold)
            return;

        //Compute the gravityscale to get the correct jump duration
        gravity = (-2 * jumpHeight) / (timeToJumpApex * timeToJumpApex * gravityScale);
        if (IsFalling)
            gravityMult = fallingGravityMultiplier;
        else
            gravityMult = 1;
        body.AddForce(Vector3.up * gravity * gravityMult, ForceMode.Acceleration);

        //Drag horizontal only (to not mess up jump height)
        Vector3 velocity = body.velocity;
        // float dragFactor = Mathf.Pow(1f - drag * Time.fixedDeltaTime, Time.fixedDeltaTime);
        float dragFactor = 1 - drag * Time.fixedDeltaTime;
        velocity.x *= dragFactor;

        //Clamp falling velocity
        velocity.y = Mathf.Max(velocity.y, -maxFallSpeed);
        body.velocity = velocity;
    }

    private void JumpPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        isChargingJump = true;
        StartCoroutine(ChargeJump());
    }

    private void JumpReleased(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        ReleaseJump();
    }

    private void OnGrabHold()
    {
        body.velocity = Vector3.zero;
    }

    IEnumerator ChargeJump()
    {
        body.velocity = Vector3.zero;

        hasPreCharged = false;
        preJumpTime01 = 0;
        jumpCharge01 = 0;
        //while (preJumpTime01 < 1)
        //{
        //    preJumpTime01 += Time.deltaTime / minChargeTime;
        //    yield return null;
        //}

        //hasPreCharged = true;
        ////Add the overshoot of pre charge time
        //jumpCharge01 = ((preJumpTime01 - 1) * minChargeTime) / fullChargeTime;
        hasPreCharged = true;
        while (jumpCharge01 < 1)
        {
            jumpCharge01 += Time.deltaTime / fullChargeTime;
            yield return null;
        }
    }

    void ReleaseJump()
    {
        StopAllCoroutines();
        isChargingJump = false;

        if (hasPreCharged)
            Jump(GetJumpDirection());
    }

    void Jump(Vector3 direction)
    {
        isChargingJump = false;
        hasPreCharged = false;
        holdGrabber.ReleaseHold();
        body.AddForce(direction * JumpForce, ForceMode.VelocityChange);
        onJump.Invoke();
    }

    Vector3 GetJumpDirection()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane plane = Tower.Instance.GetPlaneOfPosition(transform.position);
        Vector3 raycastPoint;
        if (plane.Raycast(ray, out float distance))
        {
            raycastPoint = ray.origin + ray.direction * distance;
        }
        else
        {
            Debug.LogError("Couldn't raycast the plane for input");
            return Vector3.up;
        }

        // Vector2 playerPosVS = cam.WorldToViewportPoint(transform.position);
        // Vector2 mousePosVS = cam.ScreenToViewportPoint(Mouse.current.position.ReadValue());

        // return (mousePosVS - playerPosVS).normalized;
        return (raycastPoint - transform.position).normalized;
    }

    //Vector3 ConvertJumpDirToWorld(Vector2 dir)
    //{
    //    //Vertical force component
    //    Vector3 worldJumpDir = Vector3.up * dir.y;
    //    //Horizontal force component
    //    worldJumpDir += Tower.Instance.GetTangeant(transform.position) * dir.x;
    //    return worldJumpDir;
    //}

    private void OnDrawGizmos()
    {
        if (Tower.Instance == null)
            return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Tower.Instance.GetTangeant(transform.position));

        if (isChargingJump)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(Tower.Instance.GetPositionAtDistance(transform.position, 0.47f), 0.1f);
            Vector3 dir = GetJumpDirection();
            if (hasPreCharged)
            {
                Gizmos.color = Color.blue;
            }
            else
            {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawRay(transform.position, dir);
            Gizmos.DrawSphere(transform.position, 0.1f * jumpCharge01);
        }
    }
}
