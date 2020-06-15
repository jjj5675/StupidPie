using System.Collections.Generic;
using UnityEngine;

public class Observer
{
    private Unsubscriber m_Cancellation;
    private PlayerDataBase m_PlayerInfo;

    public PlayerDataBase PlayerInfo { get { return m_PlayerInfo; } }

    public class Unsubscriber
    {
        private List<Observer> m_observers;
        private Observer m_observer;

        internal Unsubscriber(List<Observer> observers, Observer observer)
        {
            this.m_observers = observers;
            this.m_observer = observer;
        }

        public void Dispose()
        {
            if (m_observers.Contains(m_observer))
            {
                m_observers.Remove(m_observer);
            }
        }
    }

    public Observer(PlayerDataBase playerInfo)
    {
        this.m_PlayerInfo = playerInfo;
    }

    public virtual void Subscribe(Publisher publisher)
    {
        m_Cancellation = publisher.Subscribe(this);
    }

    public virtual void Unsubscribe()
    {
        m_Cancellation.Dispose();
    }

    public virtual void OnCompleted()
    {

    }

    public void InputControl(bool gain)
    {
        if (m_PlayerInfo.playerInput == null)
        {
            return;
        }

        if (gain)
        {
            m_PlayerInfo.playerInput.GainControl();
        }
        else
        {
            m_PlayerInfo.playerInput.ReleaseControl(true);
        }
    }

    public void SetObserver(bool resetHealth, bool resetTrigger, int hashPara, Transform entrance)
    {
        if (resetHealth)
        {
            m_PlayerInfo.damageable.SetHealth(m_PlayerInfo.damageable.startingHealth);
        }

        if (resetTrigger)
        {
            m_PlayerInfo.animator.SetTrigger(hashPara);
        }

        m_PlayerInfo.transform.position = entrance.position;
        m_PlayerInfo.transform.rotation = entrance.rotation;
    }
}

