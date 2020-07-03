using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class DebugUI : MonoBehaviour
{
    public CellController cellController;
    public ScreenManager screenManager;

    public TMP_Dropdown dropdown;

    protected List<string> m_CellNames = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (var cell in cellController.CellCache)
        {
            m_CellNames.Add(cell.name);
        }

        dropdown.AddOptions(m_CellNames);
    }

    public void Move()
    {
        int index = dropdown.value - 1;

        if (m_CellNames.Contains(cellController.CellCache[index].name))
        {
            if (cellController.CellCache[index] == cellController.CurrentCell)
            {
                return;
            }

            cellController.SetCell(cellController.CellCache[index], CellTransitionDestination.DestinationTag.A);
            screenManager.autoCameraSetup.SetMainConfinerBound(cellController.CurrentCell.confinerCollider);
            Publisher.Instance.SetObservers(false, true, cellController.LastEnteringDestination.locations);
            cellController.DisablePreviousCell();
        }
    }
}