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

    List<Cell> m_CellCache = new List<Cell>();
    Cell m_CurrentCell;
    Cell m_PreviousCell;

    public Cell CurrentCell { get { return m_CurrentCell; } }

    CellTransitionDestination.DestinationTag m_FirstEnteringTag;


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

    public Transform GetCharacterLocation(GameObject character, CellTransitionDestination.DestinationTag destinationTag)
    {
        CellTransitionDestination entrance;
        CurrentCell.GetCellDestination(destinationTag, out entrance);

        if(character == transitioningSeri)
        {
            return entrance.seriLocation.transform;
        }
        else
        {
            return entrance.iresLocation.transform;
        }
    }

    public void SetCells(Cell newCell)
    {
        if(newCell == null || !m_CellCache.Contains(newCell))
        {
            Debug.LogWarning("new cell information has not been set");
            return;
        }

        m_PreviousCell = m_CurrentCell;
        m_CurrentCell = newCell;
        m_CurrentCell.gameObject.SetActive(true);

        AutoCameraSetup.Instance.SwapVirtualCamera(newCell.confinerCollider);
    }

    public void OnDisabledPreviousCell()
    {
        m_PreviousCell.gameObject.SetActive(false);
    }
}
