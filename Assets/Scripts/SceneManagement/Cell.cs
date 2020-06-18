using UnityEngine;

public class Cell : MonoBehaviour
{
    [HideInInspector]
    public CellTransitionDestination[] cellTransitionDestinations = new CellTransitionDestination[7];
    public CompositeCollider2D confinerCollider;

    protected Platform[] m_PlatformCache;
    protected ScorePickup[] m_ScorePickups;
    protected Bounds m_ConfinerBounds;

    public Bounds ConfinerBounds { get { return m_ConfinerBounds; } }

    void Awake()
    {
        cellTransitionDestinations = GetComponentsInChildren<CellTransitionDestination>();
        m_PlatformCache = GetComponentsInChildren<Platform>();
        m_ScorePickups = GetComponentsInChildren<ScorePickup>();
        m_ConfinerBounds = confinerCollider.bounds;
    }

    public void GetCellDestination(CellTransitionDestination.DestinationTag destinationTag, out CellTransitionDestination cellTransitionDestination)
    {
        for (int i = 0; i < cellTransitionDestinations.Length; i++)
        {
            if (cellTransitionDestinations[i].destinationTag == destinationTag)
            {
                cellTransitionDestination = cellTransitionDestinations[i];
                return;
            }
        }

        cellTransitionDestination = null;
        Debug.LogError("해당 목적지가 현재 셀에 없습니다");
    }

    public void ResetCell(bool transition)
    {
        for (int i = 0; i < m_PlatformCache.Length; i++)
        {
            if (m_PlatformCache[i] != null)
            {
                m_PlatformCache[i].ResetPlatform();
            }
        }

        for (int i = 0; i < m_ScorePickups.Length; i++)
        {
            if (m_ScorePickups[i] != null)
            {
                if(m_ScorePickups[i].pickupState == ScorePickup.PickupState.GAIN)
                {
                    if(transition)
                    {
                        m_ScorePickups[i].scoreData.SaveData();
                        m_ScorePickups[i] = null;
                    }
                    else
                    {
                        m_ScorePickups[i].pickupState = ScorePickup.PickupState.NOT_GAIN;
                        m_ScorePickups[i].onEnableScore.Invoke();
                        m_ScorePickups[i].scoreData.ResetData();
                    }
                }
            }
        }
    }
}
