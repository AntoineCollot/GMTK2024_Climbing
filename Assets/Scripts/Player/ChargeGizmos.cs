using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeGizmos : MonoBehaviour
{
    const int DOT_COUNT = 6;
    [SerializeField] float distanceBetweenDots = 0.075f;
    [SerializeField] ChargeGizmosDot dotPrefab;
    ChargeGizmosDot[] dots;

    PlayerJumpController jumpController;
    PlayerClimbingHoldGrabber holdGrabber;

    // Start is called before the first frame update
    void Start()
    {
        jumpController = FindObjectOfType<PlayerJumpController>();
        holdGrabber = jumpController.GetComponent<PlayerClimbingHoldGrabber>();
        jumpController.onStartChargingJump.AddListener(OnStartChargingJump);
        holdGrabber.onReleaseHold.AddListener(OnReleaseHold);

        dots = new ChargeGizmosDot[DOT_COUNT];
        for (int i = 0; i < DOT_COUNT; i++)
        {
            dots[i] = Instantiate(dotPrefab, transform);
            dots[i].Init(i);
            dots[i].SetVisible(false);
        }
    }

    private void OnReleaseHold(ClimbingHold hold)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].Hide();
        }
    }

    private void OnStartChargingJump()
    {
        StartCoroutine(UpdateGizmos());
    }

    IEnumerator UpdateGizmos()
    {
        while (jumpController.isChargingJump)
        {
            Vector3[] positions = jumpController.PreComputeJumpPositions(DOT_COUNT, distanceBetweenDots);

            int visibleDotCount = Mathf.FloorToInt(Mathf.Lerp(0, positions.Length, jumpController.jumpCharge01));
            visibleDotCount = Mathf.Min(visibleDotCount, dots.Length);
            for (int i = 0; i < dots.Length; i++)
            {
                bool isVisible = i < visibleDotCount;
                dots[i].SetVisible(isVisible);
                if(isVisible)
                    dots[i].UpdateDot(in positions[i], positions.Length);
            }

            yield return null;
        }
    }
}