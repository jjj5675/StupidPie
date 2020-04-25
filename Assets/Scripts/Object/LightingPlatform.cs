using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LightingPlatform : MonoBehaviour
{
    public PlatformCatcher platformCatcher;
    public UnityEvent Caughted;
    public UnityEvent UnCaughted;

    protected bool m_EventFired = false;

    void FixedUpdate()
    {
        if (platformCatcher.CaughtIresCharacter)
        {
            if (!m_EventFired)
            {
                Caughted.Invoke();
                m_EventFired = true;
            }
        }
        else
        {
            if(m_EventFired)
            {
                UnCaughted.Invoke();
                m_EventFired = false;
            }
        }
    }
}
