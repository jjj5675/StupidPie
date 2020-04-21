using UnityEngine;

public class SceneTransitionDestination : MonoBehaviour
{
    public enum DestinationTag
    { A, B, C }


    public DestinationTag destinationTag;
    public GameObject transitioningGameObject;
}
