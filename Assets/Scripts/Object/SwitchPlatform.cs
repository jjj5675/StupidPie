using UnityEngine;

public class SwitchPlatform : Platform
{
    public float toggletime;
    public EdgeCollider2D edgeCollider;
    public LayerMask overlapColliderMask;

    protected ContactFilter2D m_OverlapCharacterContactFilter;
    protected Collider2D m_Collider;
    protected float m_CurrentTime = 0;
    protected bool m_OnTrigger = true;
    protected bool m_CanChangeSwitch = true;

    void Awake()
    {
    }

    protected override void Initialise()
    {
        if(edgeCollider == null)
        {
            edgeCollider = GetComponent<EdgeCollider2D>();
        }

        m_PlatformType = PlatformType.SWITCH;
        edgeCollider.enabled = false;

        m_OverlapCharacterContactFilter.layerMask = overlapColliderMask;
        m_OverlapCharacterContactFilter.useLayerMask = true;
        m_OverlapCharacterContactFilter.useTriggers = false;
    }

    public void EnableOnSwitch()
    {
        m_CanChangeSwitch = true;
    }

    public void DisableOnSwitch()
    {
        m_CanChangeSwitch = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!m_CanChangeSwitch)
        {
            return;
        }

        m_CurrentTime += Time.deltaTime;

        //swtich On Off
        if (toggletime <= m_CurrentTime)
        {
            m_OnTrigger = !m_OnTrigger;
            m_CurrentTime = 0;

            if (m_OnTrigger)
            {
                SearchOverlapCharacter(edgeCollider, m_OverlapCharacterContactFilter, 5);
                m_Collider.isTrigger = false;
            }
            else
            {
                m_Collider.isTrigger = true;
            }
        }
    }
}
