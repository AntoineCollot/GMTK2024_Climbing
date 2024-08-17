using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum GlobalSFX
{
    Jump,
    Land,
    EnemyDeath,
    CatKill,
    EagleKill,
    CatAttack,
    EagleAttack,
    Hit,
    Slide,
    Kick,
    Dive,
    DiveLanding,
    PlayerDeath,
    ButtonClick,
    ProjectileExplosion,
    LevelCleared,
}
public class SFXManager : MonoBehaviour
{
    public SFXBank bank;

    [HideInInspector] public GlobalSFX lastSFX;
    [HideInInspector] public float lastSFXTime;
    public const float MIN_SFX_INTERVAL = 0.1f;

    AudioSource audioSource = null;

    public static SFXManager Instance;

    // Start is called before the first frame update
    void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(GlobalSFX sfx)
    {
        if (Instance == null || (Instance.lastSFX == sfx && Time.time - Instance.lastSFXTime < MIN_SFX_INTERVAL))
            return;

        PlaySound((int)sfx);
        Instance.lastSFX = sfx;
        Instance.lastSFXTime = Time.time;
    }

    public static void PlaySound(int id)
    {
        if (Instance == null)
            return;
        Instance.audioSource.PlayOneShot(Instance.bank.clips[id], Instance.bank.volumes[id]);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SFXManager))]
public class SFXManagerEditor : Editor
{
    bool showSFXList = true;
    SerializedObject sfxBankObj;
    SerializedProperty clips;
    SerializedProperty volumes;

    private void OnEnable()
    {
        sfxBankObj = new SerializedObject(((SFXManager)target).bank);
        clips = sfxBankObj.FindProperty("clips");
        volumes = sfxBankObj.FindProperty("volumes");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        SFXManager sfxManager = (SFXManager)target;

        showSFXList = EditorGUILayout.Foldout(showSFXList, "Global SFX");
        if(showSFXList && sfxManager.bank!=null)
        {
            SFXBankEditor.ShowClips(clips, volumes,sfxManager.bank);
        }
        if(sfxBankObj!=null)
            sfxBankObj.ApplyModifiedProperties();

        GUILayout.Space(10);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Space(10);

        DrawDefaultInspector();
    }
}
#endif