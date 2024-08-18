using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlaceAroundTowerHelper : MonoBehaviour
{
#if UNITY_EDITOR
    Tower tower;
    [SerializeField] float distFromCenter = 0.5f;
    [SerializeField] bool inverseLookAt;

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
        if (transform.position == Vector3.zero)
            return;
        if (tower == null)
            tower = FindObjectOfType<Tower>();

        transform.position = tower.GetPositionAtDistance(transform.position, distFromCenter);

        Vector3 lookAt = tower.GetTowerCenter(transform.position.y);
        if (inverseLookAt)
            lookAt = transform.position * 2 - lookAt;
        transform.LookAt(lookAt);
    }

    private void OnValidate()
    {
        UpdatePos();
    }
#endif
}
