using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnims : MonoBehaviour
{
    [Header("Rotations")]
    [SerializeField] Transform model;

    [Header("Anims")]
    Animator anim;
    PlayerJumpController jumpController;
    PlayerClimbingHoldGrabber holdGrabber;
    bool isChargingJumpAnim;

    Rigidbody body;

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
    }

    private void Update()
    {
        model.LookAt(Tower.Instance.GetTowerCenter(model.position.y));

        if(jumpController.isChargingJump != isChargingJumpAnim)
        {
            isChargingJumpAnim = jumpController.isChargingJump;
            anim.SetBool("IsCharging", isChargingJumpAnim);
        }
    }

    private void OnReleaseHold(ClimbimgHold hold)
    {
        anim.SetBool("IsGrabbing", false);
    }

    private void OnGrabHold(ClimbimgHold hold)
    {
        //Vector3 toHoldPos = hold.grabbedPosition - holdGrabber.GrabPosition;
        //bool isLeft = Vector3.Cross(toHoldPos, model.right).y > 0;
        Vector3 hVelocity = body.velocity;
        hVelocity.y = 0;
        bool isLeft = Vector3.Cross(hVelocity, Tower.Instance.GetDirectionFromCenter(transform.position)).y < 0;
        if (isLeft)
            anim.SetFloat("GrabDirection", -1);
        else
            anim.SetFloat("GrabDirection", 1);

        anim.SetBool("IsGrabbing", true);
    }

    private void OnJump()
    {
        anim.SetTrigger("Jump");
    }
}
