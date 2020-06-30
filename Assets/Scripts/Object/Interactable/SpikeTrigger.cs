using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

[RequireComponent(typeof(Damager))]
[RequireComponent(typeof(BoxCollider2D))]
public class SpikeTrigger : Platform
{
    public Damager damager;
    //public UnityEvent OnEabled;
    //public UnityEvent OnDisabled;

    protected BoxCollider2D m_Box;
    protected SpikeTrigger[] m_SpikeTriggers;
    protected List<SpikeTrigger> m_SameSpikes = new List<SpikeTrigger>();
    protected bool m_IgnoreTrigger = false;
    protected Animator m_Animator;
    protected readonly int m_HashTriggerEnablePara = Animator.StringToHash("TriggerEnable");
    protected readonly int m_HashTriggerDisablePara = Animator.StringToHash("TriggerDisable");

    public bool ChangeOnce { get; set; } = false;

    void Awake()
    {
        m_Box = GetComponent<BoxCollider2D>();
        m_Animator = GetComponentInChildren<Animator>();
    }

    protected override void Initialise()
    {
        if (damager == null)
        {
            damager = GetComponent<Damager>();
        }

        SearchOverlapPlatforms(m_Box, out m_SpikeTriggers, 30);

        for(int i=0; i<m_SpikeTriggers.Length; i++)
        {
            if(m_SpikeTriggers[i].isMovingAtStart == isMovingAtStart)
            {
                m_SameSpikes.Add(m_SpikeTriggers[i]);
            }
        }

        if (isMovingAtStart)
        {
            m_Started = true;
            damager.EnableOnDamage();
        }
        else
        {
            m_Started = false;
            damager.DisableOnDamage();
        }

        m_PlatformType = PlatformType.SPIKE_TRIGGER;
        m_CurrentTriggerState = TriggerState.EXIT;
        m_Box.isTrigger = true;
    }

    public override void ResetPlatform()
    {
        //trigger enter이면서 exit가 아닐때
        if(m_Started)
        {
            damager.EnableOnDamage();
        }
        else
        {
            damager.DisableOnDamage();
        }

        m_CurrentTriggerState = TriggerState.EXIT;
    }

    public bool Resettable
    {
        get
        {
            foreach (var spike in m_SameSpikes)
            {
                if(spike.m_CurrentTriggerState != TriggerState.EXIT)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public override void StartMoving()
    {
        if (m_Started)
        {
            DisableOverlapDamagers(true, true, true);
        }
        else
        {
            EnableOverlapDamagers(true, true, true);
        }
    }

    public override void StopMoving()
    {
        ChangeOnce = false;

        if (m_Started)
        {
            EnableOverlapDamagers(false, false);
        }
        else
        {
            DisableOverlapDamagers(false, false);
        }
    }

    public void EnableOverlapDamagers(bool enableEvent, bool change = true, bool ignoreTrigger = false)
    {
        if (ChangeOnce)
        {
            return;
        }

        for (int i = 0; i < m_SpikeTriggers.Length; i++)
        {
            if (enableEvent)
            {
                m_SpikeTriggers[i].m_Animator.SetTrigger(m_SpikeTriggers[i].m_HashTriggerEnablePara);
                //m_SpikeTriggers[i].OnEabled.Invoke();
            }
            else
            {
                m_SpikeTriggers[i].m_Animator.SetTrigger(m_SpikeTriggers[i].m_HashTriggerDisablePara);
                //m_SpikeTriggers[i].OnDisabled.Invoke();
            }

            if (m_SameSpikes.Contains(m_SpikeTriggers[i]))
            {
                m_SpikeTriggers[i].damager.EnableOnDamage();
            }
            else
            {
                m_SpikeTriggers[i].damager.DisableOnDamage();
            }

            m_SpikeTriggers[i].ChangeOnce = change;
            m_SpikeTriggers[i].m_IgnoreTrigger = ignoreTrigger;
        }
    }

    public void DisableOverlapDamagers(bool enableEvent, bool change = true, bool ignoreTrigger = false)
    {
        if (ChangeOnce)
        {
            return;
        }

        for (int i = 0; i < m_SpikeTriggers.Length; i++)
        {
            if (enableEvent)
            {
                m_SpikeTriggers[i].m_Animator.SetTrigger(m_SpikeTriggers[i].m_HashTriggerEnablePara);
                //m_SpikeTriggers[i].OnEabled.Invoke();
            }
            else
            {
                m_SpikeTriggers[i].m_Animator.SetTrigger(m_SpikeTriggers[i].m_HashTriggerDisablePara);
                //m_SpikeTriggers[i].OnDisabled.Invoke();
            }

            if(m_SameSpikes.Contains(m_SpikeTriggers[i]))
            {
                m_SpikeTriggers[i].damager.DisableOnDamage();
            }
            else
            {
                m_SpikeTriggers[i].damager.EnableOnDamage();
            }

            m_SpikeTriggers[i].ChangeOnce = change;
            m_SpikeTriggers[i].m_IgnoreTrigger = ignoreTrigger;
        }
    }

    //바꾸기
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (m_IgnoreTrigger)
        {
            return;
        }

        if (Publisher.Instance.TryGetObserver(collider, out Observer observer))
        {
            if (observer.PlayerInfo.abilityTypes.Contains(PlayerDataBase.AbilityType.INTERACTION))
            {
                m_CurrentTriggerState = TriggerState.ENTER;

                if (m_Started)
                {
                    DisableOverlapDamagers(true);
                }
                else
                {
                    EnableOverlapDamagers(true);
                }
            }
        }
    }

    //원상복귀
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (m_IgnoreTrigger)
        {
            return;
        }

        if (Publisher.Instance.TryGetObserver(collider, out Observer observer))
        {
            if (observer.PlayerInfo.abilityTypes.Contains(PlayerDataBase.AbilityType.INTERACTION))
            {
                m_CurrentTriggerState = TriggerState.EXIT;

                if (Resettable)
                {
                    ChangeOnce = false;

                    if (m_Started)
                    {
                        EnableOverlapDamagers(false, false);
                    }
                    else
                    {
                        DisableOverlapDamagers(false, false);
                    }
                }
            }
        }
    }

}
