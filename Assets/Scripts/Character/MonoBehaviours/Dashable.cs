using System;
using UnityEngine;
using UnityEngine.Events;

public class Dashable : MonoBehaviour
{
    [Serializable]
    public class DashEvent : UnityEvent<Dashable> { }

    public enum DashState { Ready, Dashing, AirborneDashing, Cooldown }

    public DashEvent OnDash;

    private float m_DashunableTimer;
    private DashState m_CurrentDashState;

    public DashState CurrentDashState { get { return m_CurrentDashState; } }

    void OnEnable()
    {
        SetState(DashState.Ready);
    }

    void Update()
    {
        //cooldown 끼임bug
        if (m_CurrentDashState == DashState.Dashing || m_CurrentDashState == DashState.AirborneDashing)
        {
            m_DashunableTimer -= Time.deltaTime;

            if (m_DashunableTimer <= 0)
            {
                SetState(DashState.Cooldown);
            }
        }
    }

    public void DisableDashability()
    {
        SetState(DashState.Cooldown);
    }

    public void EnableDashability(float duration, bool grounded)
    {
        if (grounded)
        {
            SetState(DashState.Dashing);
        }
        else
        {
            SetState(DashState.AirborneDashing);
        }

        m_DashunableTimer = duration;
    }

    public void UpdateDashable()
    {
        if (m_CurrentDashState != DashState.Ready)
        {
            return;
        }

        OnDash.Invoke(this);
    }

    public void SetState(DashState state)
    {
        m_CurrentDashState = state;
    }
}
