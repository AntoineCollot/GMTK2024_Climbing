using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingHoldRing : ClimbingHold
{
   const float RADIUS = 0.5f;

    public override Vector3 GetGrabPosition(Vector3 from)
    {
        Vector3 dir = Tower.GetDirectionFromCenter(from);
        return Tower.GetTowerCenter(transform.position.y) + dir * RADIUS;
    }
}
