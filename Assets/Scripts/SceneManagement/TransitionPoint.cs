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

    int m_TransitioningPresentCount = 0;
    bool m_TransitioningPresent = false;


    private void OnDisable()
    {
        if(m_TransitioningPresent)
        {
            m_TransitioningPresent = false;
            publisher.GainOrReleaseControl(true);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(publisher.ColliderHasObserver(collision))
        {
            m_TransitioningPresentCount++;
        }

        if(publisher.Observers.Count <= m_TransitioningPresentCount && transitionWhen == TransitionWhen.OnTriggerEnter)
        {
            cellController.SetCell(transitionCell, transitionDestinationTag);
            screenManager.autoCameraSetup.SwapVCam(cellController.CurrentCell.confinerCollider);
            publisher.GainOrReleaseControl(false);
            m_TransitioningPresent = true;

            TransitionInternal();
        }
    }

    //void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (publisher.ColliderHasObserver(collision))
    //    {
    //        m_TransitioningPresentCount--;
    //    }
    //}

    void TransitionInternal()
    {
        if(transitionType == TransitionType.SameScene)
        {
            publisher.SetObservers(false, true, cellController.LastEnteringDestination.locations);
        }
    }
}
