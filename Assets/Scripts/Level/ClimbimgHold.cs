using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbimgHold : MonoBehaviour
{
    public Vector3 grabbedPosition { get; private set; }
    public bool isGrabbed { get; private set; }

    //Add a little offset for the hand doesn't clip
    public const float HAND_Y_OFFSET = 0.002f;

    public virtual float DistanceFrom(Vector3 position)
    {
        return Vector3.Distance(position, GetGrabPosition(position));
    }

    public virtual Vector3 GetGrabPosition(Vector3 from)
    {
        return transform.position + Vector3.up * HAND_Y_OFFSET;
    }

    public virtual void Grabbed(Vector3 from)
    {
        isGrabbed = true;
        grabbedPosition = GetGrabPosition(from);
    }

    public virtual void Released()
    {
        isGrabbed=false;
    }
}
