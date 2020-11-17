using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class EndMove : MonoBehaviour
{
    
    bool canMove = false;

    void Update()
    {
        if(canMove && Input.anyKeyDown)
        {
            StartCoroutine(Load());
        }
    }
    public void Ended()
    {
        canMove = true;
    }
    IEnumerator Load()
    {
        yield return StartCoroutine(ScreenFader.FadeSceneOut());
        SceneManager.LoadSceneAsync("Credit");
        StartCoroutine(ScreenFader.FadeSceneIn());

        yield return null;
    }
}
