using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindEffector : MonoBehaviour
{
    bool applyWind;
    TowerSegment segment;
    PlayerJumpController player;
    bool isInit;

    private void Start()
    {
        segment = GetComponent<TowerSegment>();
        player = GameManager.Instance.player.GetComponent<PlayerJumpController>();
        GameManager.Instance.onPlayerChangeFloor.AddListener(OnPlayerFloorChange);
        isInit = true;
    }

    private void OnEnable()
    {
        //Don't use on enable on start to avoid script exec issues with game manager
        if (isInit && GameManager.Instance != null)
            GameManager.Instance.onPlayerChangeFloor.AddListener(OnPlayerFloorChange);
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.onPlayerChangeFloor.RemoveListener(OnPlayerFloorChange);
    }

    private void OnPlayerFloorChange(int from, int to)
    {
        applyWind = to == segment.floor;
    }

    private void FixedUpdate()
    {
        if (!applyWind)
            return;

        player.EnableWindForNextFrame();
    }
}
