using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    [SerializeField] GameObject cloudPrefab;
    [SerializeField] int cloudCount;
    List<Transform> clouds;
    [SerializeField] float cloudDistance = 5;

    // Start is called before the first frame update
    void Start()
    {
        clouds = new List<Transform>();
        //Init clouds on first two floors
        for (int i = 0; i < cloudCount; i++)
        {
            Transform newCloud = Instantiate(cloudPrefab, transform).transform;
            if (i < cloudCount / 2)
                newCloud.position = GetCloudPosition(0);
            else
                newCloud.position = GetCloudPosition(1);

            newCloud.LookAt(Tower.GetTowerCenter(newCloud.position.y));
            clouds.Add(newCloud);
        }

        GameManager.Instance.onPlayerChangeFloor.AddListener(OnPlayerChangedFloor);
    }

    private void OnPlayerChangedFloor(int from, int to)
    {
        if (clouds == null)
            return;

        bool isGoingUp = from < to;
        for (int i = 0; i < cloudCount; i++)
        {
            int cloudFloor = Tower.GetFloorOfAltitude(clouds[i].position.y);
            if (isGoingUp)
            {
                //If a cloud is in a floor too far
                if (cloudFloor < from)
                {
                    //move it up
                    clouds[i].position = GetCloudPosition(to + 1);
                }
            }
            else
            {
                if (cloudFloor > from)
                {
                    //move it down
                    clouds[i].position = GetCloudPosition(to - 1);
                }
            }

            clouds[i].LookAt(Tower.GetTowerCenter(clouds[i].position.y));
        }
    }

    Vector3 GetCloudPosition(int floor)
    {
        float altitude = Random.Range(0f, (float)Tower.FLOOR_HEIGHT) + floor * Tower.FLOOR_HEIGHT;
        Vector3 dir = Random.rotation * Vector3.forward;
        dir.y = 0;
        dir.Normalize();
        return dir * cloudDistance + Vector3.up * altitude;
    }
}
