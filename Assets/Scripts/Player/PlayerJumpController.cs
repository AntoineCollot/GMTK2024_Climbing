using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerJumpController : MonoBehaviour
{
    [Header("Jump")]
    [SerializeField, Range(0, 1)] float smallJumpFactor = 0.3f;
    [SerializeField, Range(0.5f, 5)] float jumpHeight = 0.5f;
    [SerializeField, Range(0.2f, 2)] float timeToJumpApex = 0.5f;
    [SerializeField, Range(0, 1)] float sideJumpFactor = 0.5f;

    [Header("Charge")]
    [SerializeField] float fullChargeTime;
    public float jumpCharge01 { get; private set; }
    public bool isChargingJump { get; private set; }

    [Header("Fall")]
    [SerializeField] float gravityScale = 3.5f;
    float gravity;
    [SerializeField] float fallingGravityMultiplier = 1;
    public float maxFallSpeed;
    [SerializeField, Range(0, 1f)] float drag;

    [Header("Wind")]
    [SerializeField] float windStrength;
    bool enableWindForNextFrame;

    PlayerClimbingHoldGrabber holdGrabber;
    PlayerStickToTower stickToTower;

    InputMap inputs;
    Rigidbody body;
    Camera cam;

    public UnityEvent onStartChargingJump = new UnityEvent();
    public UnityEvent onJump = new UnityEvent();

    public bool IsFalling => body.velocity.y < -0.01f;
    public float JumpForce => GetJumpForce(gravity, jumpCharge01);

    // Start is called before the first frame update
    void Start()
    {
        holdGrabber = GetComponentInChildren<PlayerClimbingHoldGrabber>();
        stickToTower = GetComponentInChildren<PlayerStickToTower>();
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
        if (isChargingJump || holdGrabber.isGrabbingHold || !GameManager.Instance.GameIsPlaying)
            return;

        gravity = ComputeGravity();
        //if (IsFalling)
        //    gravityMult = fallingGravityMultiplier;
        //else
        //    gravityMult = 1;
        //body.AddForce(Vector3.up * gravity * gravityMult, ForceMode.Acceleration);

        //Vector3 velocity = body.velocity;

        ////Add wind velocity
        //if (enableWindForNextFrame)
        //{
        //    //Add most wind if not already moving in this direction, to avoid wind snowballing
        //    Vector3 windDir = -Tower.GetTangeant(body.position);
        //    Vector3 velocityByWind = velocity;
        //    if (velocityByWind.y < 0)
        //        velocityByWind.y = 0;

        //    float alreadyInWind = Mathf.Clamp01(Vector3.Dot(windDir, velocityByWind.normalized));
        //    velocityByWind.y = 0;
        //    alreadyInWind *= velocityByWind.magnitude;
        //    velocity += windDir * Mathf.Lerp(windStrength, windStrength * 0.35f, alreadyInWind) * Time.fixedDeltaTime;
        //    enableWindForNextFrame = false;
        //}

        ////Drag horizontal only (to not mess up jump height)
        //velocity.x *= DragFactor;
        //velocity.z *= DragFactor;

        ////Clamp falling velocity
        //velocity.y = Mathf.Max(velocity.y, -maxFallSpeed);
        body.velocity = ComputeNewVelocity(body.velocity, body.position, IsFalling, gravity, fallingGravityMultiplier, maxFallSpeed, drag, windStrength, enableWindForNextFrame, Time.fixedDeltaTime);
        enableWindForNextFrame = false;
    }

#if UNITY_EDITOR
    public bool alwaysAllowJump;
#endif
    private void JumpPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!GameManager.Instance.GameIsPlaying)
            return;

        bool canJump = holdGrabber.isGrabbingHold;
#if UNITY_EDITOR
        if (alwaysAllowJump)
            canJump = true;
#endif
        if (canJump)
        {
            StartChargingJump();
        }
    }

    void StartChargingJump()
    {
        isChargingJump = true;
        StartCoroutine(ChargeJump());
        onStartChargingJump.Invoke();
    }

    private void JumpReleased(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (isChargingJump)
            ReleaseJump();
    }

    private void OnGrabHold(ClimbingHold hold)
    {
        body.velocity = Vector3.zero;
    }

    IEnumerator ChargeJump()
    {
        body.velocity = Vector3.zero;

        jumpCharge01 = 0;
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

        Jump(GetJumpDirection());
    }

    void Jump(Vector3 direction)
    {
        isChargingJump = false;
        holdGrabber.ReleaseHold();
        Vector3 force = CombineJumpDirAndForce(in direction, JumpForce);
        body.AddForce(force, ForceMode.VelocityChange);
        onJump.Invoke();
    }

    public Vector3 GetJumpDirection()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane plane = Tower.GetPlaneOfPosition(transform.position);
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

        // return (mousePosVS - playerPosVS).normalized;
        return (raycastPoint - transform.position).normalized;
    }

    float GetJumpForce(float gravity, float jumpCharge01)
    {
        return Mathf.Sqrt(-2f * gravity * jumpHeight) * Mathf.Lerp(smallJumpFactor, 1, jumpCharge01);
    }

    Vector3 CombineJumpDirAndForce(in Vector3 jumpDirection, float jumpForce)
    {
        //How much are we going horizontal
        float horizontalAmount = 1 - Mathf.Abs(Vector3.Dot(jumpDirection, Vector3.up));
        horizontalAmount = Mathf.Clamp01(horizontalAmount.Remap(0.1f, 0.7f, 0, 1));
        //Reduce the force based on horizontal amount
        jumpForce *= Mathf.Lerp(1, sideJumpFactor, horizontalAmount);
        return jumpDirection * jumpForce;
    }

    float ComputeGravity()
    {
        //Compute the gravityscale to get the correct jump duration
        return (-2 * jumpHeight) / (timeToJumpApex * timeToJumpApex * gravityScale);
    }

    public Vector3[] PreComputeJumpPositions(int samples, float distanceBetweenSamples)
    {
        return PreComputeJumpPositions(samples, distanceBetweenSamples, transform.position, GetJumpDirection(), jumpCharge01, Time.fixedDeltaTime);
    }

    public void EnableWindForNextFrame()
    {
        enableWindForNextFrame = true;
    }

    static Vector3 ComputeNewVelocity(Vector3 velocity, Vector3 position, bool isFalling, float gravity, float fallingGravityMultiplier, float maxFallSpeed, float drag, float windStrength, bool enableWind, float deltaTime)
    {
        float gravityMult;
        if (isFalling)
            gravityMult = fallingGravityMultiplier;
        else
            gravityMult = 1;

        velocity += Vector3.up * gravity * gravityMult * deltaTime;

        //Add wind velocity
        if (enableWind)
        {
            //Add most wind if not already moving in this direction, to avoid wind snowballing
            Vector3 windDir = -Tower.GetTangeant(position);
            Vector3 velocityByWind = velocity;
            if (velocityByWind.y < 0)
                velocityByWind.y = 0;

            float alreadyInWind = Mathf.Clamp01(Vector3.Dot(windDir, velocityByWind.normalized));
            velocityByWind.y = 0;
            alreadyInWind *= velocityByWind.magnitude;
            velocity += windDir * Mathf.Lerp(windStrength, windStrength * 0.35f, alreadyInWind) * Time.fixedDeltaTime;
        }

        //Drag horizontal only (to not mess up jump height)
        velocity.x *= 1 - drag * Time.fixedDeltaTime;
        velocity.z *= 1 - drag * Time.fixedDeltaTime;

        //Clamp falling velocity
        velocity.y = Mathf.Max(velocity.y, -maxFallSpeed);

        return velocity;
    }

    Vector3[] PreComputeJumpPositions(int samples, float distanceBetweenSamples, Vector3 origin, Vector3 jumpDirection, float jumpCharge01, float deltaTime)
    {
        //Init velocity from jump
        float gravity = ComputeGravity();
        Vector3 velocity = CombineJumpDirAndForce(in jumpDirection, GetJumpForce(gravity, jumpCharge01));
        Vector3 position = origin;

        Vector3[] positions = new Vector3[samples];
        int positionID = 0;
        float distanceTraveledFromLastSample = 0;

        //Loop for all frames
        while (positionID < samples)
        {
            Vector3 nextPos = position;

            velocity = ComputeNewVelocity(velocity, nextPos, velocity.y < -0.01f, gravity, fallingGravityMultiplier, maxFallSpeed, drag, windStrength, enableWindForNextFrame, deltaTime);
            nextPos += velocity * deltaTime;

            //Stick to tower
            stickToTower.ComputePositionAndVelocity(ref nextPos, ref velocity);

            float newDistanceTraveled = distanceTraveledFromLastSample + Vector3.Distance(position, nextPos);

            if (newDistanceTraveled > distanceBetweenSamples)
            {
                //Lerp between frames for precision
                positions[positionID] = Vector3.Lerp(position, nextPos, Mathf.InverseLerp(distanceTraveledFromLastSample, newDistanceTraveled, distanceBetweenSamples));
                positionID++;

                newDistanceTraveled -= distanceBetweenSamples;
            }
            distanceTraveledFromLastSample = newDistanceTraveled;
            position = nextPos;
        }

        return positions;
    }

#if UNITY_EDITOR
    //private void OnDrawGizmos()
    //{
    //    if (Tower.Instance == null)
    //        return;
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawRay(transform.position, Tower.Instance.GetTangeant(transform.position));

    //    if (isChargingJump)
    //    {
    //        Gizmos.color = Color.yellow;
    //        Gizmos.DrawSphere(Tower.Instance.GetPositionAtDistance(transform.position, 0.47f), 0.1f);
    //        Vector3 dir = GetJumpDirection();

    //        Gizmos.color = Color.blue;
    //        Gizmos.DrawRay(transform.position, dir);
    //        Gizmos.DrawSphere(transform.position, 0.1f * jumpCharge01);
    //    }
    //}

    Vector3[] preComputedPosDebug;
    public int debugMaxSamples;
    public float debugInterval = 0.05f;

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
            return;

        int samples = Mathf.FloorToInt(Mathf.Lerp(0, debugMaxSamples, jumpCharge01));
        if (!holdGrabber.isGrabbingHold || !isChargingJump)
            return;
        preComputedPosDebug = PreComputeJumpPositions(debugMaxSamples, debugInterval, transform.position, GetJumpDirection(), jumpCharge01, 0.01f);
        if (preComputedPosDebug != null)
        {
            samples = Mathf.Min(samples, preComputedPosDebug.Length);
            for (int i = 0; i < samples; i++)
            {
                float t = (float)(i + 1) / debugMaxSamples;
                Gizmos.color = Color.Lerp(Color.yellow, Color.red, t);
                Gizmos.DrawSphere(preComputedPosDebug[i], Mathf.Lerp(0.008f, 0.013f, t));
            }
        }
    }
#endif
}
