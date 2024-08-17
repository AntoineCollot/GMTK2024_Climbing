using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbimgHold : MonoBehaviour
{
    public float DistanceFrom(Vector3 position)
    {
        return Vector3.Distance(position, GetGrabPosition(position));
    }

    public Vector3 GetGrabPosition(Vector3 from)
    {
        return transform.position;
    }
}
