using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellController : MonoBehaviour
{
    static private CellController s_Instance;
    static public CellController Instance { get { return s_Instance; } }

    public Cell rootCell;
    public GameObject transitioningSeri;
    public GameObject transitioningIres;
    public CellTransitionDestination lastEnteringDestination;

    List<Cell> m_CellCache = new List<Cell>();
    Cell m_CurrentCell;
    Cell m_PreviousCell;

    public Cell CurrentCell { get { return m_CurrentCell; } }

    void Awake()
    {
        s_Instance = this;

        if(rootCell == null)
        {
            Debug.LogError("Root Cell is has not been set");
        }

        m_CurrentCell = rootCell;
        PopulateCellList(ref m_CellCache);
    }

    void PopulateCellList<TComponent>(ref List<TComponent> list)
        where TComponent :  Component
    {
        TComponent[] components = FindObjectsOfType<TComponent>();

        for(int i = 0; i<components.Length; i++)
        {
            list.Add(components[i]);

            if(components[i]!=rootCell)
            {
                components[i].gameObject.SetActive(false);
            }
        }
    }

    public void SetCells(Cell newCell, CellTransitionDestination.DestinationTag destinationTag)
    {
        if(newCell == null || !m_CellCache.Contains(newCell))
        {
            Debug.LogWarning("new cell information has not been set");
            return;
        }

        m_PreviousCell = m_CurrentCell;
        m_CurrentCell = newCell;
        m_CurrentCell.gameObject.SetActive(true);
        m_CurrentCell.GetCellDestination(destinationTag, out lastEnteringDestination);

        AutoCameraSetup.Instance.SwapVirtualCamera(newCell.confinerCollider);
    }

    public void OnDisabledPreviousCell()
    {
        if (m_PreviousCell != null)
        {
            m_PreviousCell.ResetPlatformInCell();
            m_PreviousCell.gameObject.SetActive(false);
        }
    }
}
