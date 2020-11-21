using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class StartUI : MonoBehaviour
{
    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void Credit()
    {
        StartCoroutine(CreditMove());
    }
    IEnumerator CreditMove()
    {
        yield return StartCoroutine(ScreenFader.FadeSceneOut());
        SceneManager.LoadSceneAsync("Credit");
        StartCoroutine(ScreenFader.FadeSceneIn());

        yield return null;
    }
}

