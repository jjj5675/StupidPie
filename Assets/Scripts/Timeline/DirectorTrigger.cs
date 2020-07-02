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
        Invoke("FinishInvoke", (float)director.duration);
    }

    void FinishInvoke()
    {
        OnDirectorFinish.Invoke();
    }

}
