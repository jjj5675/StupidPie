using System.Collections.Generic;
using UnityEngine;

public class Publisher : MonoBehaviour
{
    private List<Observer> m_Observers;

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

    public void GainOrReleaseControl(bool gain)
    {
        foreach(var observer in m_Observers)
        {
            observer.InputControl(gain);
        }
    }
}

