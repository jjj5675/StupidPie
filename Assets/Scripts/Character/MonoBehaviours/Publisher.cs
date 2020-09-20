using System.Collections.Generic;
using UnityEngine;

//퍼블리셔는 저장된 옵저버 들에 대한 전체적인 통제를 담당하는 클래스.
public class Publisher : MonoBehaviour
{
    static private Publisher s_Instance;

    static public Publisher Instance { get { return s_Instance; } }

    private List<Observer> m_Observers = new List<Observer>();

    public List<Observer> Observers { get { return m_Observers; } }

    //싱글톤
    private void Awake()
    {
        s_Instance = this;
    }

    //옵저버를 추가하는 구문. 
    public Observer.Unsubscriber Subscribe(Observer observer)
    {
        if (!m_Observers.Contains(observer))
        {
            m_Observers.Add(observer);
        }

        return new Observer.Unsubscriber(m_Observers, observer);
    }


    //충돌 검출을 위한 함수. 충돌한 컬라이더가 플레이어의 것인지 판단한다.
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

    public bool TryGetObserver(Collider2D collider, out Observer observer)
    {
        foreach (var o in m_Observers)
        {
            if(o.PlayerInfo.collider == collider)
            {
                observer = o;
                return true;
            }
        }

        observer = null;
        return false;
    }

    public void GainOrReleaseControl(bool gain)
    {
        foreach(var observer in m_Observers)
        {
            PlayerInput[] inp;
            inp = observer.GetInput();
            foreach (PlayerInput inps in inp)
            {
                if (gain)
                {
                    inps.GainControl();
                }
                else
                {
                    inps.ReleaseControl(true);
                }
            }
        }
    }

    public void GainPause()
    {
        foreach(var observer in m_Observers)
        {
            PlayerInput[] inp;
            inp = observer.GetInput();
            foreach (PlayerInput inps in inp)
            {
                inps.Pause.GainControl();
            }
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

