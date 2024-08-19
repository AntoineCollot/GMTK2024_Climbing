using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    bool isLoading = false;

    public void LoadNext()
    {
        if (isLoading)
            return;

        isLoading = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Retry()
    {
        if (isLoading)
            return;

        isLoading = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMenu()
    {
        if (isLoading)
            return;

        isLoading = true;
        SceneManager.LoadScene(1);
    }
}
