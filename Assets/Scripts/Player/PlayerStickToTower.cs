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
        Vector3 velocity = body.velocity;
        Vector3 position = body.position;

        ComputePositionAndVelocity(ref position, ref velocity);

        body.position = position;
        body.velocity = velocity;
    }

    private void LateUpdate()
    {
        //Move the player to the correct distance from center
        transform.position = Tower.GetPositionAtDistance(transform.position,distanceFromCenter);
    }

    public void ComputePositionAndVelocity(ref Vector3 position, ref Vector3 velocity)
    {
        //Move the player to the correct distance from center
        Vector3 newPos = Tower.GetPositionAtDistance(position, distanceFromCenter);

        Vector3 projectedVelocity = Vector3.ProjectOnPlane(velocity, Tower.GetDirectionFromCenter(position));
        projectedVelocity.Normalize();

        //Apply the same magnitude along the plane
        velocity = projectedVelocity * velocity.magnitude;
        position = newPos;
    }
}
