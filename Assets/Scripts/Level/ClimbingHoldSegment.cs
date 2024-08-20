using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingHoldSegment : ClimbingHold
{
    [SerializeField] float segmentYOffset;
    [SerializeField] float segmentWidth;

    Vector3 SegmentCenter => transform.position + Vector3.up * (segmentYOffset + HAND_Y_OFFSET);
    Vector3 SegmentA => SegmentCenter - transform.right * segmentWidth * 0.5f;
    Vector3 SegmentB => SegmentCenter + transform.right * segmentWidth * 0.5f;

    public override Vector3 GetGrabPosition(Vector3 from)
    {
        return GetClosestPointOnLineSegment(SegmentA, SegmentB, from);
    }

    public static Vector3 GetClosestPointOnLineSegment(Vector3 segmentA, Vector3 segmentB, Vector3 point)
    {
        Vector3 AP = point - segmentA;       //Vector from A to P   
        Vector3 AB = segmentB - segmentA;       //Vector from A to B  

        float ABAPproduct = Vector3.Dot(AP, AB);    //The DOT product of a_to_p and a_to_b     
        float distance = ABAPproduct / AB.sqrMagnitude; //The normalized "distance" from a to your closest point  

        if (distance < 0)     //Check if P projection is over vectorAB     
            return segmentA;
        else if (distance > 1)
            return segmentB;
        else
            return segmentA + AB * distance;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(SegmentA, SegmentB);
    }
#endif
}
