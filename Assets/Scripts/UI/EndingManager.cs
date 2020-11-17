using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingManager : MonoBehaviour
{
   void Start()
    {

        StartCoroutine(ScreenFader.FadeSceneIn());
    }
}
