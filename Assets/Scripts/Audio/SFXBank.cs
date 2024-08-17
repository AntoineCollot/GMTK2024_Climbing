using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "NewSFXBank", menuName = "SFXBank", order = 1)]
public class SFXBank : ScriptableObject
{
    [HideInInspector]
    public List<AudioClip> clips = new List<AudioClip>();
    [HideInInspector]
    public List<float> volumes = new List<float>();
}

#if UNITY_EDITOR
[CustomEditor(typeof(SFXBank))]
public class SFXBankEditor : Editor
{
    SerializedProperty clips;
    SerializedProperty volumes;

    private void OnEnable()
    {
        clips = serializedObject.FindProperty("clips");
        volumes = serializedObject.FindProperty("volumes");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        SFXBank sfxBank = (SFXBank)target;

        ShowClips(clips, volumes,sfxBank);
        serializedObject.ApplyModifiedProperties();
    }

    public static void ShowClips(SerializedProperty clips,SerializedProperty volumes, SFXBank target)
    {
        int sfxClipCount = System.Enum.GetNames(typeof(GlobalSFX)).Length;
        EditorGUI.indentLevel += 1;
        GUIStyle style = new GUIStyle("window");
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.UpperLeft;
        Color defaultColor = GUI.backgroundColor;
        Color boxColor = new Color(1, 0.8f, 0.2f, 1);

        for (int i = 0; i < sfxClipCount; i++)
        {
            GUILayout.Space(5);
            //Draw a box
            GUI.backgroundColor = boxColor;
            GUILayout.BeginVertical(((GlobalSFX)i).ToString(), style);

            //Add a clip if missing
            while (i >= target.clips.Count)
            {
                target.clips.Add(null);
            }
            while (i >= target.volumes.Count)
            {
                target.volumes.Add(0.5f);
            }

            GUILayout.Space(5);
            EditorGUILayout.PropertyField(clips.GetArrayElementAtIndex(i), GUIContent.none);
            EditorGUILayout.Slider(volumes.GetArrayElementAtIndex(i), 0, 1, "Volume");

            GUILayout.EndVertical();
            GUI.backgroundColor = defaultColor;
        }

        //Remove overflow clips
        while (target.clips.Count > sfxClipCount)
        {
            target.clips.RemoveAt(target.clips.Count - 1);
        }
        while (target.volumes.Count > sfxClipCount)
        {
            target.volumes.RemoveAt(target.volumes.Count - 1);
        }

        EditorGUI.indentLevel -= 1;
    }
}
#endif