using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LightingPlatform : MonoBehaviour
{
    public bool isMovingAtStart;
    public PlatformCatcher platformCatcher;
    public UnityEvent Caughted;
    public UnityEvent UnCaughted;

    protected bool m_Started = false;

    void Start()
    {
        if (platformCatcher == null)
        {
            platformCatcher = GetComponent<PlatformCatcher>();
        }

        Initialise();
    }

    protected void Initialise()
    {
        if (isMovingAtStart)
        {
            m_Started = true;
        }
        else
        {
            m_Started = false;
        }
    }

    void FixedUpdate()
    {
        if(!m_Started && !platformCatcher.CaughtIresCharacter)
        {
            UnCaughted.Invoke();
            return;
        }

        Caughted.Invoke();
    }

    public void StartMoving()
    {
        m_Started = true;
    }

    public void StopMoving()
    {
        m_Started = false;
    }
}
