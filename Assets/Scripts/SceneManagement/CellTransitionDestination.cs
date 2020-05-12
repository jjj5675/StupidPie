using UnityEngine;

public class CellTransitionDestination : MonoBehaviour
{
    public enum DestinationTag
    {
        A, B, C, D, E, F, G
    }

    public DestinationTag destinationTag;

    public GameObject seriLocation;
    public GameObject iresLocation;
}
