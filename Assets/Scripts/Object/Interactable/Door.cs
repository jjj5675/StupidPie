using UnityEngine;
using System.Collections.Generic;

public class Door : MonoBehaviour
{
    public PressurePad[] pressurePads;
    public AudioSource DoorOpen;
    public AudioSource DoorClose;

    Collider2D m_DoorCollider;
    Animator m_Animator;
    bool m_EventFired;
    ContactFilter2D m_ContactFilter = new ContactFilter2D();
    Collider2D[] m_OverlapBuffer = new Collider2D[5];

    protected readonly int m_HashOpenPara = Animator.StringToHash("Opened");
    protected readonly int m_HashClosePara = Animator.StringToHash("Closed");

    bool ReleaseAllPads
    {
        get
        {
            for (int i = 0; i < pressurePads.Length; i++)
            {
                if (pressurePads[i].EventFired)
                {
                    return false;
                }
            }

            return true;
        }
    }

    private void Awake()
    {
        m_Animator = GetComponentInChildren<Animator>();
        m_DoorCollider = GetComponent<Collider2D>();
        m_ContactFilter.useLayerMask = true;
        m_ContactFilter.useTriggers = true;
        m_ContactFilter.layerMask = 1 << LayerMask.NameToLayer("Player");
        Physics2D.queriesStartInColliders = false;
    }

    public void UnLock()
    {
        if (!m_EventFired)
        {
            m_Animator.SetTrigger(m_HashOpenPara);
            m_EventFired = true;
        }
    }

    public void Lock()
    {
        if (m_EventFired && ReleaseAllPads)
        {
            m_Animator.SetTrigger(m_HashClosePara);
            m_EventFired = false;
        }
    }


    private void Update()
    {
        if (m_EventFired && 0.99f <= m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime)
        {
            m_DoorCollider.isTrigger = true;
        }
        else if (!m_EventFired && m_DoorCollider.isTrigger)
        {
            int count = Physics2D.OverlapCollider(m_DoorCollider, m_ContactFilter, m_OverlapBuffer);

            for (int i = 0; i < count; i++)
            {
                if(Publisher.Instance.TryGetObserver(m_OverlapBuffer[i], out Observer observer))
                {
                    observer.PlayerInfo.damageable.TakeDamage(null);
                }
            }

            m_DoorCollider.isTrigger = false;

        }
    }
}
