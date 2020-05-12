using UnityEngine;

public class SwitchPlatform : Platform
{
    public Damager damager;
    public float toggletime;
    public ContactFilter2D contactFilter;

    protected Collider2D m_Collider;
    protected float m_CurrentTime = 0;
    protected bool m_OnTrigger = true;
    protected Collider2D[] m_FoundCollider = new Collider2D[5]; 

    protected override void Initialise()
    {
        if (damager == null)
        {
            damager = GetComponent<Damager>();
        }

        m_Collider = GetComponent<Collider2D>();

        damager.enabled = false;
        m_PlatformType = PlatformType.SWITCH;
    }

    // Update is called once per frame
    void Update()
    {
        m_CurrentTime += Time.deltaTime;

        //swtich On Off
        if (toggletime <= m_CurrentTime)
        {
            m_OnTrigger = !m_OnTrigger;
            m_CurrentTime = 0;
            damager.enabled = false;
            m_Collider.isTrigger = false;

            if (m_OnTrigger)
            {
                Vector2 rayStart = (Vector2)transform.position + m_Collider.offset;
                Vector3 scaledSize = Vector2.Scale(damager.size, transform.lossyScale);

                int hitCount = Physics2D.OverlapBox(rayStart, scaledSize, 0, contactFilter, m_FoundCollider);

                if (hitCount != 0)
                {
                    damager.enabled = true;
                    m_Collider.isTrigger = true;
                }
            }
            else
            {
                m_Collider.isTrigger = true;
            }
        }
    }
}
