using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Damager))]
[RequireComponent(typeof(BoxCollider2D))]
public class SpikeTrigger : Platform
{
    public Damager damager;
    public UnityEvent OnEabled;
    public UnityEvent OnDisabled;

    protected BoxCollider2D m_Box;
    protected SpikeTrigger[] m_SpikeTriggers;
    protected bool m_IgnoreTrigger = false;

    public bool ChangeOnce { get; set; } = false;

    void Awake()
    {
        m_Box = GetComponent<BoxCollider2D>();
    }

    protected override void Initialise()
    {
        if (damager == null)
        {
            damager = GetComponent<Damager>();
        }

        SearchOverlapPlatforms(m_Box, out m_SpikeTriggers, 30);

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
        //m_Box.isTrigger = true;
        //OnDisabled.Invoke();
    }

    public bool Resettable
    {
        get
        {
            for (int i = 0; i < m_SpikeTriggers.Length; i++)
            {
                if (m_SpikeTriggers[i].m_CurrentTriggerState != TriggerState.EXIT)
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
                m_SpikeTriggers[i].OnEabled.Invoke();
            }
            else
            {
                m_SpikeTriggers[i].OnDisabled.Invoke();
            }

            m_SpikeTriggers[i].damager.EnableOnDamage();
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
                m_SpikeTriggers[i].OnEabled.Invoke();
            }
            else
            {
                m_SpikeTriggers[i].OnDisabled.Invoke();
            }

            m_SpikeTriggers[i].damager.DisableOnDamage();
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

        if (collider.GetComponent<PlayerBehaviour>().dataBase.abilityTypes.Contains(PlayerDataBase.AbilityType.GIMMICK_ACTIVATE))
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

    //원상복귀
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (m_IgnoreTrigger)
        {
            return;
        }

        if (collider.GetComponent<PlayerBehaviour>().dataBase.abilityTypes.Contains(PlayerDataBase.AbilityType.GIMMICK_ACTIVATE))
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
