using UnityEngine;
using UnityEngine.Tilemaps;

public class TriggerPlatform : Platform
{
    public CompositeCollider2D compositeCollider;
    public EdgeCollider2D edgeCollider;
    public LayerMask overlapColliderMask;
    public TileBase onTileset;
    public TileBase offTileset;

    protected ContactFilter2D m_OverlapCharacterContactFilter;
    protected SpikeTrigger[] m_SpikeTriggers;
    protected Tilemap m_Tilemap;

    void Awake()
    {
        m_Tilemap = GetComponent<Tilemap>();
    }

    protected override void Initialise()
    {
        if (compositeCollider == null)
        {
            compositeCollider = GetComponent<CompositeCollider2D>();
        }

        if (edgeCollider == null)
        {
            edgeCollider = GetComponent<EdgeCollider2D>();
        }


        edgeCollider.enabled = false;
        SearchOverlapPlatforms(compositeCollider, out m_SpikeTriggers, 30, false);

        if (isMovingAtStart)
        {
            m_Started = true;
            compositeCollider.isTrigger = false;
        }
        else
        {
            m_Started = false;
            compositeCollider.isTrigger = true;
        }

        m_OverlapCharacterContactFilter.layerMask = overlapColliderMask;
        m_OverlapCharacterContactFilter.useLayerMask = true;
        m_OverlapCharacterContactFilter.useTriggers = false;
    }

    public override void ResetPlatform()
    {
    }

    public override void StartMoving()
    {
        EnableOverlapSpikeTriggers();

        if(!m_Started)
        {
            SearchOverlapCharacter(edgeCollider, m_OverlapCharacterContactFilter, 5);
        }
    }

    public override void StopMoving()
    {
        DisableOverlapSpikeTriggers();

        if(m_Started)
        {
            SearchOverlapCharacter(edgeCollider, m_OverlapCharacterContactFilter, 5);
        }
    }

    void EnableOverlapSpikeTriggers()
    {
        foreach(var pos in m_Tilemap.cellBounds.allPositionsWithin)
        {
            if(m_Tilemap.HasTile(pos))
            {
                m_Tilemap.SetTile(pos, onTileset);
            }
        }

        if (m_Started)
        {
            compositeCollider.isTrigger = true;
        }
        else
        {
            compositeCollider.isTrigger = false;
        }

        if (m_SpikeTriggers.Length != 0)
        {
            for (int i = 0; i < m_SpikeTriggers.Length; i++)
            {
                m_SpikeTriggers[i].StartMoving();
            }
        }
    }

    void DisableOverlapSpikeTriggers()
    {
        foreach (var pos in m_Tilemap.cellBounds.allPositionsWithin)
        {
            if (m_Tilemap.HasTile(pos))
            {
                m_Tilemap.SetTile(pos, offTileset);
            }
        }

        if (m_Started)
        {
            compositeCollider.isTrigger = false;
        }
        else
        {
            compositeCollider.isTrigger = true;
        }

        if (m_SpikeTriggers.Length != 0)
        {
            m_SpikeTriggers[0].ChangeOnce = false;

            for (int i = 0; i < m_SpikeTriggers.Length; i++)
            {
                m_SpikeTriggers[i].StopMoving();
            }
        }
    }

    //ON -> OFF
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!Publisher.Instance.TryGetObserver(collision.collider, out Observer observer))
        {
            return;
        }

        if(!observer.PlayerInfo.abilityTypes.Contains(PlayerDataBase.AbilityType.INTERACTION))
        {
            return;
        }    

        if (!m_Started)
        {
            return;
        }

        EnableOverlapSpikeTriggers();
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (!Publisher.Instance.TryGetObserver(collider, out Observer observer))
        {
            return;
        }

        if (!observer.PlayerInfo.abilityTypes.Contains(PlayerDataBase.AbilityType.INTERACTION))
        {
            return;
        }

        if (!m_Started)
        {
            return;
        }

        //데미지
        SearchOverlapCharacter(edgeCollider, m_OverlapCharacterContactFilter, 5);
        DisableOverlapSpikeTriggers();
    }

    //OFF -> ON
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!Publisher.Instance.TryGetObserver(collider, out Observer observer))
        {
            return;
        }

        if (!observer.PlayerInfo.abilityTypes.Contains(PlayerDataBase.AbilityType.INTERACTION))
        {
            return;
        }

        if (m_Started)
        {
            return;
        }

        //데미지
        SearchOverlapCharacter(edgeCollider, m_OverlapCharacterContactFilter, 5);
        EnableOverlapSpikeTriggers();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(!Publisher.Instance.TryGetObserver(collision.collider, out Observer observer))
        {
            return;
        }

        if(!observer.PlayerInfo.abilityTypes.Contains(PlayerDataBase.AbilityType.INTERACTION))
        {
            return;
        }   

        if (m_Started)
        {
            return;
        }

        DisableOverlapSpikeTriggers();
    }
}
