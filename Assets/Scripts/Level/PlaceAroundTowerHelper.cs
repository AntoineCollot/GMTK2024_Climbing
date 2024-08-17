using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlaceAroundTowerHelper : MonoBehaviour
{
#if UNITY_EDITOR
    Tower tower;
    [SerializeField] float distFromCenter;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdatePos();
    }

    void UpdatePos()
    {
        if (tower == null)
            tower = FindObjectOfType<Tower>();

        transform.position = tower.GetPositionAtDistance(transform.position, distFromCenter);

        transform.LookAt(tower.GetTowerCenter(transform.position.y));
    }

    private void OnValidate()
    {
        UpdatePos();
    }
#endif
}
