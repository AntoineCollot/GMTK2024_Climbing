using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public static Tower Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetTowerCenter(float altitude)
    {
        return Vector3.up * altitude;
    }

    public Vector3 GetDirectionFromCenter(Vector3 pos)
    {
        pos = pos - GetTowerCenter(pos.y);
        pos.y = 0;
        return pos.normalized;
    }

    public Vector3 GetPositionAtDistance(Vector3 pos, float distance)
    {
        Vector3 center = GetTowerCenter(pos.y);
        Vector3 toPos = pos - center;
        toPos.y = 0;
        toPos.Normalize();
        return center + toPos * distance;
    }

    public Vector3 GetTangeant(Vector3 pos)
    {
        return Vector3.Cross(GetDirectionFromCenter(pos), Vector3.up);
    }

    public Plane GetPlaneOfPosition(Vector3 pos)
    {
        Vector3 planePoint = pos;
        Vector3 planeNormal = GetDirectionFromCenter(pos);

        return new Plane(planeNormal, planePoint);
    }
}
