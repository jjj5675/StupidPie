using System;
using UnityEngine;
using UnityEngine.Events;

public class Damager : MonoBehaviour
{
    public int damage;
    public Vector2 offset;
    public Vector2 size;
    public LayerMask hitLayerMask;

    protected bool m_CanDamage = true;
    protected ContactFilter2D m_HitContactFilter;
    protected Collider2D[] m_HitOverlapResults = new Collider2D[10];
    protected Collider2D m_LastHit;

    void Awake()
    {
        m_HitContactFilter.layerMask = hitLayerMask;
        m_HitContactFilter.useLayerMask = true;
        m_HitContactFilter.useTriggers = false;
    }

    public void EnableOnDamage()
    {
        m_CanDamage = true;
    }

    public void DisableOnDamage()
    {
        m_CanDamage = false;
    }

    void FixedUpdate()
    {
        if (!m_CanDamage)
        {
            return;
        }

        Vector2 scale = transform.lossyScale;

        Vector2 facingOffset = Vector2.Scale(offset, scale);
        Vector2 scaledSize = Vector2.Scale(size, scale);

        Vector2 pointA = (Vector2)transform.position + facingOffset - scaledSize * 0.5f;   //min
        Vector2 pointB = pointA + scaledSize;   //max

        int hitCount = Physics2D.OverlapArea(pointA, pointB, m_HitContactFilter, m_HitOverlapResults);

        for (int i = 0; i < hitCount; i++)
        {
            m_LastHit = m_HitOverlapResults[i];
            Damageable damageable = m_LastHit.GetComponent<Damageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(this);
            }
        }
    }
}
