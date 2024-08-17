using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashSceneLoadNextScene : MonoBehaviour
{
    public float delay = 3.5f;
    public float fadeLogoTime = 1f;
    public float fadeBackgroundTime = 1f;
    const float MIN_LOAD_TIME = 0.5f;

    [SerializeField] Graphic logo;

    CanvasGroup group;

    // Start is called before the first frame update
    void Start()
    {
        group = GetComponent<CanvasGroup>();
        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(delay);
        float t = 0;
        Color color = Color.white;

        while (t < 1)
        {
            t += Time.deltaTime / fadeLogoTime;

            color.a = Curves.QuadEaseInOut(1, 0, Mathf.Clamp01(t));
            logo.color = color;

            yield return null;
        }

        //Load the scene.
        //If it takes less than a second, wait for the second to finish so the animation doesn't get rushed
        float realTimeLoadStart = Time.realtimeSinceStartup;
        SceneManager.LoadScene(1,LoadSceneMode.Additive);
        yield return new WaitUntil(() => Time.realtimeSinceStartup - realTimeLoadStart > MIN_LOAD_TIME);

        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / fadeBackgroundTime;

            group.alpha = Curves.QuadEaseInOut(1, 0, Mathf.Clamp01(t));

            yield return null;
        }

        SceneManager.UnloadSceneAsync(0);
    }
}
