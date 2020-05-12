using UnityEngine;
using UnityEngine.Events;

public class LightingPlatform : Platform
{
    public PlatformCatcher platformCatcher;
    public UnityEvent Caughted;
    public UnityEvent UnCaughted;

    protected override void Initialise()
    {
        if (platformCatcher == null)
        {
            platformCatcher = GetComponent<PlatformCatcher>();
        }

        if (isMovingAtStart)
        {
            m_Started = true;
        }
        else
        {
            m_Started = false;
        }

        m_PlatformType = PlatformType.LIGHTING;
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
}
