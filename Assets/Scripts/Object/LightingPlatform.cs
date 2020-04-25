using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LightingPlatform : MonoBehaviour
{
    public PlatformCatcher platformCatcher;
    public Sprite lightOffSprite;
    public Sprite lightOnSprite;
    public UnityEvent Caughted;
    public UnityEvent UnCaughted;

    protected 

    void FixedUpdate()
    {
        if(platformCatcher.CaughtIresCharacter)
        {

        }
    }
}
