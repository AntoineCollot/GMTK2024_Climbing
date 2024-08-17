using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStickToTower : MonoBehaviour
{
    Rigidbody body;
    [SerializeField] float distanceFromCenter;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        //Move the player to the correct distance from center
        body.MovePosition(Tower.Instance.GetPositionAtDistance(transform.position,distanceFromCenter));

       // Plane plane = Tower.Instance.GetPlaneOfPosition(transform.position, distanceFromCenter);
        Vector3 projectedVelocity = Vector3.ProjectOnPlane(body.velocity, Tower.Instance.GetDirectionFromCenter(body.position));
        projectedVelocity.Normalize();

        //Apply the same magnitude along the plane
        body.velocity = projectedVelocity * body.velocity.magnitude;
    }

    private void LateUpdate()
    {
        //Move the player to the correct distance from center
        transform.position = Tower.Instance.GetPositionAtDistance(transform.position,distanceFromCenter);
    }

    private void Update()
    {
        //Visualy Rotate
    }
}
