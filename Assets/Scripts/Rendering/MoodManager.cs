using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoodManager : MonoBehaviour
{
    [SerializeField, ColorUsage(false)] Color shadowColor;
    [SerializeField, ColorUsage(false)] Color glossinessColor;

    // Start is called before the first frame update
    void Start()
    {
        UpdateShaders();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        UpdateShaders();
    }
#endif

    void UpdateShaders()
    {
        Shader.SetGlobalColor("_ShadowColor", shadowColor);
        Shader.SetGlobalColor("_GlossinessColor", glossinessColor);
    }
}
