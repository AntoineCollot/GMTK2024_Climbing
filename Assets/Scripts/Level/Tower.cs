using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public static Tower Instance;

    public const int FLOOR_HEIGHT = 2;

    private void Awake()
    {
        Instance = this;
    }

    public static Vector3 GetTowerCenter(float altitude)
    {
        return Vector3.up * altitude;
    }

    public static Vector3 GetDirectionFromCenter(Vector3 pos)
    {
        pos = pos - GetTowerCenter(pos.y);
        pos.y = 0;
        return pos.normalized;
    }

    public static Vector3 GetPositionAtDistance(Vector3 pos, float distance)
    {
        Vector3 center = GetTowerCenter(pos.y);
        Vector3 toPos = pos - center;
        toPos.y = 0;
        toPos.Normalize();
        return center + toPos * distance;
    }

    public static Vector3 GetTangeant(Vector3 pos)
    {
        return Vector3.Cross(GetDirectionFromCenter(pos), Vector3.up);
    }

    public static Plane GetPlaneOfPosition(Vector3 pos)
    {
        Vector3 planePoint = pos;
        Vector3 planeNormal = GetDirectionFromCenter(pos);

        return new Plane(planeNormal, planePoint);
    }
}
