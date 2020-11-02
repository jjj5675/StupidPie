using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellController : MonoBehaviour
{
    List<Cell> m_CellCache = new List<Cell>();
    Cell m_CurrentCell;
    Cell m_PreviousCell;
    CellTransitionDestination m_LastEnteringDestination;

    public Cell CurrentCell { get { return m_CurrentCell; } }
    public Cell PreviousCell { get { return m_PreviousCell; } }
    public CellTransitionDestination LastEnteringDestination { get { return m_LastEnteringDestination; } }

    public List<Cell> CellCache { get { return m_CellCache; } }

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

    public void SetCell(Cell newCell, CellTransitionDestination.DestinationTag destinationTag)
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
    }

    public void GetRootCell(out Cell cell)
    {
        foreach(var item in m_CellCache)
        {
            if(item.cellType == Cell.CellType.ROOT)
            {
                cell = item;
                return;
            }
        }

        Debug.LogError("Root Cell이 없습니다.");
        cell = null;
    }

    public void SettleRootCell(int num)
    {
        foreach(var item in m_CellCache)
        {
            if (item.cellNum == num)
                item.cellType = Cell.CellType.ROOT;
            else
                item.cellType = Cell.CellType.NONE;
        }
    }

    public void DisablePreviousCell()
    {
        if (m_PreviousCell != null)
        {
            m_PreviousCell.ResetCell(true);
            m_PreviousCell.gameObject.SetActive(false);
        }
    }
}
