using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuActivityController : MonoBehaviour
{
    Canvas[] m_Canvases;
    GraphicRaycaster[] m_Raycasters;

    TimerUI m_TimerUI;

    public TimerUI TimerUI { get { return m_TimerUI; } }

    private void Awake()
    {
        m_Canvases = GetComponentsInChildren<Canvas>(true);
        m_Raycasters = GetComponentsInChildren<GraphicRaycaster>(true);

        for(int i=0; i<m_Canvases.Length; i++)
        {
            m_TimerUI = m_Canvases[i].GetComponent<TimerUI>();

            if(m_TimerUI != null)
            {
                break;
            }
        }
    }

    public void SetActive(bool active)
    {
        for(int i=0; i<m_Canvases.Length; i++)
        {
            m_Canvases[i].enabled = active;
        }

        for(int i=0; i<m_Raycasters.Length; i++)
        {
            m_Raycasters[i].enabled = active;
        }
    }
}
