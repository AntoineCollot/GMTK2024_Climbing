using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
        if (!gameObject.scene.IsValid())
            return;
        if (transform.position == Vector3.zero)
            return;
        if (tower == null)
            tower = FindObjectOfType<Tower>();

        transform.position = Tower.GetPositionAtDistance(transform.position, distFromCenter);

        Vector3 lookAt = Tower.GetTowerCenter(transform.position.y);
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
#if UNITY_EDITOR
[CustomEditor(typeof(PlaceAroundTowerHelper))]
[CanEditMultipleObjects]
public class PlaceAroundTowerHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlaceAroundTowerHelper helper = (PlaceAroundTowerHelper)target;
        if (!helper.gameObject.scene.IsValid())
            return;

        GUILayout.Space(10);
        GUILayout.Label("Placement", EditorStyles.boldLabel);
        float angle = Vector3.SignedAngle(Tower.GetDirectionFromCenter(helper.transform.position), Vector3.forward,Vector3.up)+180;
        GUILayout.Label($"Angle : {angle.ToString("N1")}");
        GUILayout.Label($"Height : {helper.transform.position.y.ToString("N1")}");
    }
}
#endif