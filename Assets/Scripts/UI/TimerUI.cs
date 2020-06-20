using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    public TextMeshProUGUI minuteAndSecondsText;
    public TextMeshProUGUI milliSecondsText;

    private float m_Timer;
    private int m_TotalMinutes = 0;
    public bool m_Timeable = true;

    public void ResetTimer()
    {
        m_Timer = 0;
        m_TotalMinutes = 0;
        minuteAndSecondsText.text = "00:00";
        milliSecondsText.text = ".00";
    }

    public void StopTimer()
    {
        m_Timeable = false;
    }

    public void StartTimer()
    {
        m_Timeable = true;
    }

    public void TakeTimerInfo(SettleUI settle)
    {
        settle.timerText.text += minuteAndSecondsText.text + milliSecondsText.text;
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
