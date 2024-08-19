using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSegment : MonoBehaviour
{

#if UNITY_EDITOR
    const string SOURCE_NAME ="Items";
    const string MIRROR_NAME ="Mirror";
    [ContextMenu("Mirror")]
    public void Mirror()
    {
        Transform source = null;
        foreach (Transform child in transform)
        {
            if (child.name == SOURCE_NAME)
                source = child;
            if (child.name == MIRROR_NAME)
                DestroyImmediate(child.gameObject);
        }

        if(source == null)
        {
            Debug.LogError($"No child matching name {SOURCE_NAME} to mirror");
            return;
        }

        Transform mirror = new GameObject(MIRROR_NAME).transform;
        mirror.SetParent(transform, false);

        //Copy and rotate all children
        foreach (Transform child in source)
        {
            Transform mirrorItem = Instantiate(child, mirror);
            mirrorItem.RotateAround(Vector3.zero, Vector3.up, 180);
        }
    }
#endif

}
