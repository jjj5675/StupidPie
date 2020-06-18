using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class TransitionPoint : MonoBehaviour
{
    public enum TransitionType
    { 
        DifferentZone, DifferentNonGameplayScene, SameScene
    }

    public enum TransitionWhen
    {
        ExternalCall, InteractPressed, OnTriggerEnter
    }

    public TransitionType transitionType;
    public Cell transitionCell;
    public CellTransitionDestination.DestinationTag transitionDestinationTag;
    public Publisher publisher;
    public TransitionWhen transitionWhen;
    public bool resetInputValueOnTransition = true;
    public CellController cellController;

    int transitioningPresentCount = 0;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(publisher.ColliderHasObserver(collision))
        {
            transitioningPresentCount++;
        }

        if(publisher.Observers.Count <= transitioningPresentCount && transitionWhen == TransitionWhen.OnTriggerEnter)
        {
            cellController.SetCells(transitionCell, transitionDestinationTag, true);
            TransitionInternal();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (publisher.ColliderHasObserver(collision))
        {
            transitioningPresentCount--;
        }
    }

    void TransitionInternal()
    {
        if(transitionType == TransitionType.SameScene)
        {
            publisher.SetObservers(false, false, true, cellController.LastEnteringDestination.playerLocations);
        }
    }
}
