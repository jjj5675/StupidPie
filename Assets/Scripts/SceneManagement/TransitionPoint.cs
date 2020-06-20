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

    [SceneName]
    public string newSceneName;
    public TransitionType transitionType;
    public Cell transitionCell;
    public CellTransitionDestination.DestinationTag transitionDestinationTag;
    public Publisher publisher;
    public TransitionWhen transitionWhen;
    public bool resetInputValueOnTransition = true;
    public CellController cellController;
    public ScreenManager screenManager;

    int transitioningPresentCount = 0;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(publisher.ColliderHasObserver(collision))
        {
            transitioningPresentCount++;
        }

        if(publisher.Observers.Count <= transitioningPresentCount && transitionWhen == TransitionWhen.OnTriggerEnter)
        {
            cellController.SetCell(transitionCell, transitionDestinationTag);
            screenManager.autoCameraSetup.SwapVCam(cellController.CurrentCell.confinerCollider);
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
            publisher.SetObservers(false, true, cellController.LastEnteringDestination.locations);
        }
    }
}
