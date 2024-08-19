using System.Collections;
using UnityEngine;

public class PlayerAnims : MonoBehaviour
{
    [Header("Rotations")]
    [SerializeField] Transform model;
    [SerializeField] float lookAtOffsetMaxAngle = 0.5f;
    [SerializeField] float lookAtAngleSmooth = 0.3f;
    float targetLookAtAngle;
    float lookAtOffsetAngle;
    float refLookAtOffsetAngle;

    [Header("Anims")]
    Animator anim;
    PlayerJumpController jumpController;
    PlayerClimbingHoldGrabber holdGrabber;
    bool isChargingJumpAnim;

    [Header("Fall Speed")]
    float smoothedFallSpeed;
    const float FALL_SPEED_SMOOTH = 0.3f;
    float refFallSpeed;

    Vector3 lastVelocity;
    Rigidbody body;

    int animChargeHash;
    int animVerticalSpeedHash;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        body = GetComponentInChildren<Rigidbody>();
        holdGrabber = GetComponent<PlayerClimbingHoldGrabber>();
        jumpController = GetComponent<PlayerJumpController>();

        holdGrabber.onReleaseHold.AddListener(OnReleaseHold);
        holdGrabber.onGrabHold.AddListener(OnGrabHold);
        jumpController.onJump.AddListener(OnJump);
        jumpController.onStartChargingJump.AddListener(OnStartChargingJump);

        animChargeHash = Animator.StringToHash("IsCharging");
        animVerticalSpeedHash = Animator.StringToHash("VerticalSpeed");
    }

    private void Update()
    {
        if (jumpController.isChargingJump != isChargingJumpAnim)
        {
            isChargingJumpAnim = jumpController.isChargingJump;
            anim.SetBool(animChargeHash, isChargingJumpAnim);
        }

        UpdateLookAtDirection();

        //Fall speed
        float maxFallSpeed = jumpController.maxFallSpeed;
        //Decrease max fall speed when going up, cause we slow fast so not much going at real fall speed
        if (body.velocity.y > 0.1f)
            maxFallSpeed *= 0.3f;
        smoothedFallSpeed = Mathf.SmoothDamp(smoothedFallSpeed, Mathf.Clamp(body.velocity.y / maxFallSpeed, -1, 1), ref refFallSpeed, FALL_SPEED_SMOOTH);
        anim.SetFloat(animVerticalSpeedHash, smoothedFallSpeed);
    }

    private void LateUpdate()
    {
        lastVelocity = body.velocity;
    }

    private void OnReleaseHold(ClimbingHold hold)
    {
        anim.SetBool("IsGrabbing", false);
    }

    private void OnGrabHold(ClimbingHold hold)
    {
        //Vector3 toHoldPos = hold.grabbedPosition - holdGrabber.GrabPosition;
        //bool isLeft = Vector3.Cross(toHoldPos, model.right).y > 0;
        Vector3 hVelocity = lastVelocity;
        hVelocity.y = 0;
        bool isLeft = Vector3.Cross(hVelocity, Tower.GetDirectionFromCenter(transform.position)).y < 0;
        if (isLeft)
            anim.SetFloat("GrabDirection", -1);
        else
            anim.SetFloat("GrabDirection", 1);

        anim.SetBool("IsGrabbing", true);
    }

    private void OnJump()
    {
        anim.SetTrigger("Jump");

        //Reset look at
        targetLookAtAngle = 0;
    }

    private void OnStartChargingJump()
    {
        StartCoroutine(LookAtJumpDirectionWhileCharging());
    }

    void UpdateLookAtDirection()
    {
        Vector3 lookAtPos = Tower.GetTowerCenter(model.position.y);

        lookAtOffsetAngle = Mathf.SmoothDamp(lookAtOffsetAngle, targetLookAtAngle, ref refLookAtOffsetAngle, lookAtAngleSmooth);
        lookAtPos = RotatePointAroundPivot(lookAtPos, jumpController.transform.position, Vector3.up * lookAtOffsetAngle);

        Debug.DrawLine(lookAtPos, jumpController.transform.position, Color.red);

        model.LookAt(lookAtPos);
    }

    IEnumerator LookAtJumpDirectionWhileCharging()
    {
        while (jumpController.isChargingJump)
        {
            Vector3 jumpDirection = jumpController.GetJumpDirection();
            bool isJumpingRight = Vector3.Dot(jumpDirection, Tower.GetTangeant(jumpController.transform.position)) > 0;

            //remap the dot to reach max faster
            float amount = (Mathf.Abs(Vector3.Dot(jumpDirection, Vector3.up)) - 0.66f) * 3;
            float maxAngle = lookAtOffsetMaxAngle;
            //Adjust the max angle as it looks more impactfull on the left for some reasons
            if (isJumpingRight)
                maxAngle += 10;
            else
                maxAngle -= 10;

            targetLookAtAngle = Mathf.Lerp(maxAngle, 0, amount);

            if (!isJumpingRight)
                targetLookAtAngle *= -1;

            yield return null;
        }
    }

    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }
}
