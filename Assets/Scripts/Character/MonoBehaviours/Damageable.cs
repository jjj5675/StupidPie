using System;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    [Serializable]
    public class DamageEvent : UnityEvent<Damageable, Damager> { }

    public int startingHealth;
    public float invulnerabilityDuration; 
    public DamageEvent OnTakeDamage;
    public DamageEvent OnDie;

    protected bool m_InvuInerable;
    protected float m_InvuInerabilityTimer;
    protected int m_CurrentHealth;

    public int CurrentHealth { get { return m_CurrentHealth; } }

    void OnEnable()
    {
        m_CurrentHealth = startingHealth;
        DisableInvulnerability();
    }

    void Update()
    {
        if(m_InvuInerable)
        {
            m_InvuInerabilityTimer -= Time.deltaTime;

            if(m_InvuInerabilityTimer <=0)
            {
                m_InvuInerable = false;
            }
        }
    }

    public void EnableInvulnerability()
    {
        m_InvuInerable = true;
        m_InvuInerabilityTimer = invulnerabilityDuration;
    }

    public void DisableInvulnerability()
    {
        m_InvuInerable = false;
    }

    public void TakeDamage(Damager damager)
    {
        if(m_InvuInerable || m_CurrentHealth <= 0)
        {
            return;
        }

        m_CurrentHealth -= damager.damage;
        OnTakeDamage.Invoke(this, damager);

        Debug.Log(m_CurrentHealth);

        if(m_CurrentHealth <=0)
        {
            OnDie.Invoke(this, damager);
            EnableInvulnerability();
        }
    }

    public void SetHealth(int amount)
    {
        m_CurrentHealth = amount;
    }

}
