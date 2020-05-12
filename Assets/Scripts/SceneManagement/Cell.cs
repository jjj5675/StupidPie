using UnityEngine;

public class Cell : MonoBehaviour
{
    [HideInInspector]
    public CellTransitionDestination[] cellTransitionDestinations = new CellTransitionDestination[7];
    public CompositeCollider2D confinerCollider;

    void Awake()
    {
        cellTransitionDestinations = GetComponentsInChildren<CellTransitionDestination>();
    }

    public void GetCellDestination(CellTransitionDestination.DestinationTag destinationTag, out CellTransitionDestination cellTransitionDestination)
    {
        for(int i = 0; i<cellTransitionDestinations.Length; i++)
        {
            if(cellTransitionDestinations[i].destinationTag == destinationTag)
            {
                cellTransitionDestination = cellTransitionDestinations[i];
                return;
            }
        }

        cellTransitionDestination = null;
        Debug.LogError("해당 목적지가 현재 셀에 없습니다");
    }
}
