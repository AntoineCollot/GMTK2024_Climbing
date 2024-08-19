using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTargets : MonoBehaviour
{
    Transform player;

    [Header("Look At")]
    [SerializeField] Transform lookAtTarget;
    [Header("Position")]
    [SerializeField] Transform positionTarget;
    [SerializeField] float camDistance;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 towerCenter = Tower.GetTowerCenter(player.position.y);

        //Look at
        lookAtTarget.position = towerCenter;

        //Position
        Vector3 playerDirection = player.position - towerCenter;
        playerDirection.y = 0;
        playerDirection.Normalize();
        positionTarget.position = towerCenter + playerDirection * camDistance;
    }
}
