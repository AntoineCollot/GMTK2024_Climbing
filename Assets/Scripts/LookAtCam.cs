using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCam : MonoBehaviour
{
    /// <summary>
    /// If true, the transform will ignore the Y component and stay vertical
    /// </summary>
    [Tooltip("If true, the transform will ignore the Y component and stay vertical")]
    [SerializeField] private bool _lockY = false;
    [SerializeField] private bool _inverse = false;

    Transform camT = null;

    private void Start()
    {
        camT = Camera.main.transform;
    }

    /// <summary>
    /// AR camera
    /// </summary>
    private void LateUpdate()
    {
        Vector3 targetPos = camT.position;
        if (_inverse)
            targetPos += 2 * (transform.position - targetPos);
        if (_lockY)
            targetPos.y = transform.position.y;

        transform.LookAt(targetPos);
    }
}
