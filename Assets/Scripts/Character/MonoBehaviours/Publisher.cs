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
            observer.InputControl(gain);
        }
    }

    public void SetObservers(bool resetHealth, bool resetTrigger, int hashPara, List<GameObject> entrances)
    {
        int observerCount = 0;
        foreach(var observer in m_Observers)
        {
            observer.SetObserver(resetHealth, resetTrigger, hashPara, entrances[observerCount].transform);
            observerCount++;
        }
    }

}

