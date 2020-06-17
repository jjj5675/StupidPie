using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellController : MonoBehaviour
{
    public AutoCameraSetup autoCameraSetup;

    List<Cell> m_CellCache = new List<Cell>();
    Cell m_CurrentCell;
    Cell m_PreviousCell;
    CellTransitionDestination m_LastEnteringDestination;

    public Cell CurrentCell { get { return m_CurrentCell; } }
    public CellTransitionDestination LastEnteringDestination { get { return m_LastEnteringDestination; } }

    void Awake()
    {
        PopulateCellList(ref m_CellCache);
    }

    void PopulateCellList<TComponent>(ref List<TComponent> list)
        where TComponent : Component
    {
        TComponent[] components = FindObjectsOfType<TComponent>();

        for (int i = 0; i < components.Length; i++)
        {
            list.Add(components[i]);
            components[i].gameObject.SetActive(false);
        }
    }

    public void SetCells(Cell newCell, CellTransitionDestination.DestinationTag destinationTag, bool swapVCam = false)
    {
        if (newCell == null || !m_CellCache.Contains(newCell))
        {
            Debug.LogWarning("new cell information has not been set");
            return;
        }

        if (m_CurrentCell)
        {
            m_PreviousCell = m_CurrentCell;
        }

        m_CurrentCell = newCell;
        m_CurrentCell.gameObject.SetActive(true);
        m_CurrentCell.GetCellDestination(destinationTag, out m_LastEnteringDestination);
        autoCameraSetup.SetVCam(m_CurrentCell.confinerCollider, swapVCam);
    }

    public void OnDisabledPreviousCell()
    {
        if (m_PreviousCell != null)
        {
            m_PreviousCell.ResetCell(true);
            m_PreviousCell.gameObject.SetActive(false);
        }
    }
}
