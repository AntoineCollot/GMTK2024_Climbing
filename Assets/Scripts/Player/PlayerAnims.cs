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

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        holdGrabber = GetComponent<PlayerClimbingHoldGrabber>();
        jumpController = GetComponent<PlayerJumpController>();

        holdGrabber.onReleaseHold.AddListener(OnReleaseHold);
        holdGrabber.onGrabHold.AddListener(OnGrabHold);
        jumpController.onJump.AddListener(OnJump);
    }

    private void Update()
    {
        model.LookAt(Tower.Instance.GetTowerCenter(model.position.y));
    }

    private void OnReleaseHold()
    {
        anim.SetBool("IsGrabbing", false);
    }

    private void OnGrabHold()
    {
        anim.SetBool("IsGrabbing", true);
    }

    private void OnJump()
    {
        anim.SetTrigger("Jump");
    }
}
