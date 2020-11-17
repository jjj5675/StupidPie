﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class EndingCreditFade : MonoBehaviour
{
    
    [Serializable]
    public class EndingTexts
    {
        public string title;
        public string names;
    }

    public float temp;
    public float RemainTime;
    public EndingTexts[] texts;
    private CanvasGroup canvasGroup;
    

    void Start()
    {
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        
        canvasGroup.alpha = 0;
        EndingScene();
    }

    // Update is called once per frame
    public void EndingScene()
    {
        StartCoroutine(TextChange());
    }
    IEnumerator TextChange()
    {
        
        
        yield return new WaitForSeconds(1);
        ScreenFader.FadeSceneIn();
        for (int i=0; i<texts.Length; i++)
        {
            canvasGroup.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = texts[i].title;
            texts[i].names = texts[i].names.Replace("<>", "\n");
            canvasGroup.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = texts[i].names;
            while(!Mathf.Approximately(canvasGroup.alpha, 1))
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 1, temp * Time.deltaTime);
                yield return null;
            }
            yield return new WaitForSeconds(RemainTime);
            while (!Mathf.Approximately(canvasGroup.alpha, 0))
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 0, temp * Time.deltaTime);
                yield return null;
            }
        }
        
        StartCoroutine(ScreenFader.FadeSceneOut());
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync("Start");
        StartCoroutine(ScreenFader.FadeSceneIn());

        yield return null;
    }
}