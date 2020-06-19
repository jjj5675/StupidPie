using System.Collections.Generic;
using UnityEngine;

public class Publisher : MonoBehaviour
{
    private List<Observer> m_Observers;

    public List<Observer> Observers { get { return m_Observers; } }

    public Publisher()
    {
        m_Observers = new List<Observer>();
    }

    public Observer.Unsubscriber Subscribe(Observer observer)
    {
        if (!m_Observers.Contains(observer))
        {
            m_Observers.Add(observer);
        }

        return new Observer.Unsubscriber(m_Observers, observer);
    }

    public bool ColliderHasObserver(Collider2D collider)
    {
        foreach(var observer in m_Observers)
        {
            if(observer.PlayerInfo.collider == collider)
            {
                return true;
            }
        }

        return false;
    }

    public void GainOrReleaseControl(bool gain)
    {
        foreach(var observer in m_Observers)
        {
            if(gain)
            {
                observer.GetInput().GainControl();
            }
            else
            {
                observer.GetInput().ReleaseControl(true);
            }
        }
    }

    public void GainPause()
    {
        foreach(var observer in m_Observers)
        {
            observer.GetInput().Pause.GainControl();
        }
    }

    public void SetAnimState(bool respawn, bool dead)
    {
        foreach(var observer in m_Observers)
        {
            if(respawn)
            {
                observer.PlayerInfo.animator.SetTrigger("Respawn");
            }

            if(dead)
            {
                observer.PlayerInfo.animator.SetTrigger("Dead");
            }
        }
    }

    public void SetObservers(bool resetHealth, bool transition, List<GameObject> entrances)
    {
        int observerCount = 0;
        foreach(var observer in m_Observers)
        {
            if(resetHealth)
            {
                observer.PlayerInfo.damageable.SetHealth(observer.PlayerInfo.damageable.startingHealth);
            }

            if(transition)
            {
                observer.PlayerInfo.transform.position = entrances[observerCount].transform.position;
                observer.PlayerInfo.transform.rotation = entrances[observerCount].transform.rotation;
            }

            observerCount++;
        }
    }

}

