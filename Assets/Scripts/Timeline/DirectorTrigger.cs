using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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

    public static DirectorTrigger instance=null;

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
        instance = this;
        m_AlreadTriggered = true;
        OnDirectorPlay.Invoke();
        Invoke("FinishInvoke", (float)director.duration);
    }

    void FinishInvoke()
    {
        instance = null;
        OnDirectorFinish.Invoke();
    }
}
