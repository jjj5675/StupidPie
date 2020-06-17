using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    public Text minuteAndSecondsText;
    public Text milliSecondsText;

    private float m_Timer;
    private int m_TotalMinutes = 0;
    public bool m_Timeable = true;

    public void ResetTimer()
    {
        m_TotalMinutes = 0;
        minuteAndSecondsText.text = "00:00";
        milliSecondsText.text = "00";
        m_Timeable = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_Timeable)
        {
            return;
        }

        if (m_TotalMinutes == 99)
        {
            m_Timeable = false;
        }

        m_Timer += Time.deltaTime;

        int minutes = Mathf.FloorToInt(m_Timer / 60f);

        if (m_TotalMinutes < 59)
        {
            if (m_TotalMinutes < minutes)
            {
                m_TotalMinutes++;
            }
        }
        else
        {
            if (minutes + 1 != 60)
            {
                if (m_TotalMinutes - 60 < minutes)
                {
                    m_TotalMinutes++;
                }
            }
        }

        int seconds = Mathf.FloorToInt(m_Timer % 60f);
        int milliSeconds = Mathf.FloorToInt((m_Timer * 100f) % 100f);

        minuteAndSecondsText.text = m_TotalMinutes.ToString("00") + ":" + seconds.ToString("00");
        milliSecondsText.text = "." + milliSeconds.ToString("00");
    }
}
