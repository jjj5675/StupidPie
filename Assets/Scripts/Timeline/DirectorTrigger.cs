using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

[RequireComponent(typeof(Collider2D))]
public class DirectorTrigger : MonoBehaviour
{
    public List<GameObject> triggeringGameObject;
    public PlayableDirector director;
    public UnityEvent OnDirectorPlay;
    public UnityEvent OnDirectorFinish;

    protected bool m_AlreadTriggered;
    protected Coroutine m_ActiveCoroutine;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!triggeringGameObject.Contains(collision.gameObject))
        {
            return;
        }

        if(m_AlreadTriggered)
        {
            return;
        }

        director.Play();
        m_AlreadTriggered = true;
        OnDirectorPlay.Invoke();
        m_ActiveCoroutine = StartCoroutine(Finish((float)director.duration));
    }

    IEnumerator Finish(float duration)
    {
        yield return new WaitForSeconds(duration);
        OnDirectorFinish.Invoke();
    }

    public void ActiveCoroutine(bool active, float currentTime = 0f)
    {
        if (active)
        {
            m_ActiveCoroutine = StartCoroutine(Finish((float)director.duration - currentTime));
        }
        else
        {
            if (m_ActiveCoroutine != null)
            {
                StopCoroutine(m_ActiveCoroutine);
                m_ActiveCoroutine = null;
            }
        }
    }
}
