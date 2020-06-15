using UnityEngine;
using System.Collections.Generic;

public class CellTransitionDestination : MonoBehaviour
{
    public enum DestinationTag
    {
        A, B, C, D, E, F, G
    }

    public DestinationTag destinationTag;
    public List<GameObject> playerLocations;
}
