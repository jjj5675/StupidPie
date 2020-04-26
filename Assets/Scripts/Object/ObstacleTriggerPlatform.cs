using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityScript.Scripting.Pipeline;

public class ObstacleTriggerPlatform : MonoBehaviour
{
    public bool isMovingAtStart;
    public PlatformCatcher platformCatcher;

    protected bool m_Started = false;
    protected Damager[] m_Damagers;
    protected bool m_OnEabled = false;
    protected Collider2D m_Collider;
    protected ObstacleTriggerPlatform[] m_Platforms;

    // Start is called before the first frame update
    void Start()
    {
        if (platformCatcher == null)
        {
            platformCatcher = GetComponent<PlatformCatcher>();
        }

        if(m_Collider == null)
        {
            m_Collider = GetComponent<Collider2D>();
        }

        m_Damagers = GetComponentsInChildren<Damager>();

        if(m_Damagers != null)
        {
            for(int i =0, count = m_Damagers.Length; i<count; i++)
            {
                m_Damagers[i].enabled = false;
            }
        }

        m_OnEabled = false;

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

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!m_Started && !platformCatcher.CaughtIresCharacter)
        {
            if (m_OnEabled)
            {
                for (int i = 0, count = m_Damagers.Length; i < count; i++)
                {
                    m_Damagers[i].enabled = false;
                }

                m_OnEabled = false;
            }

            return;
        }

        if(!m_OnEabled)
        {
            for(int i=0, count = m_Damagers.Length; i<count; i++)
            {
                m_Damagers[i].enabled = true;
            }

            m_OnEabled = true;
        }
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
