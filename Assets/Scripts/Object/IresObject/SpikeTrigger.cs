using UnityEngine;

[RequireComponent(typeof(Damager))]
[RequireComponent(typeof(BoxCollider2D))]
public class SpikeTrigger : Platform
{
    public Damager damager;

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
        if(damager == null)
        {
            damager = GetComponent<Damager>();
        }

        SearchOverlapPlatforms(m_Box, out m_SpikeTriggers, 30);

        if(isMovingAtStart)
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
            DisableOverlapDamagers(true, true);
        }
        else
        {
            EnableOverlapDamagers(true, true);
        }
    }

    public override void StopMoving()
    {
        if (m_Started)
        {
            EnableOverlapDamagers(false);
        }
        else
        {
            DisableOverlapDamagers(false);
        }
    }

    public void EnableOverlapDamagers(bool change = true, bool ignoreTrigger = false)
    {
        if(ChangeOnce)
        {
            return;
        }

        for(int i = 0; i<m_SpikeTriggers.Length; i++)
        {
            m_SpikeTriggers[i].damager.EnableOnDamage();
            m_SpikeTriggers[i].ChangeOnce = change;
            m_SpikeTriggers[i].m_IgnoreTrigger = ignoreTrigger;
        }
    }

    public void DisableOverlapDamagers(bool change = true, bool ignoreTrigger = false)
    {
        if (ChangeOnce)
        {
            return;
        }

        for (int i = 0; i < m_SpikeTriggers.Length; i++)
        {
            m_SpikeTriggers[i].damager.DisableOnDamage();
            m_SpikeTriggers[i].ChangeOnce = change;
            m_SpikeTriggers[i].m_IgnoreTrigger = ignoreTrigger;
        }
    }

    //바꾸기
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(m_IgnoreTrigger)
        {
            return;
        }

        if (collider == PlayableCharacterFactory.TryGetCollider(PlayerBehaviour.PlayableCharacter.IRES))
        {
            m_CurrentTriggerState = TriggerState.ENTER;

            if (m_Started)
            {
                DisableOverlapDamagers();
            }
            else
            {
                EnableOverlapDamagers();
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

        if (collider == PlayableCharacterFactory.TryGetCollider(PlayerBehaviour.PlayableCharacter.IRES))
        {
            m_CurrentTriggerState = TriggerState.EXIT;

            if (Resettable)
            {
                ChangeOnce = false;

                if (m_Started)
                {
                    EnableOverlapDamagers(false);
                }
                else
                {
                    DisableOverlapDamagers(false);
                }
            }
        }
    }

}
