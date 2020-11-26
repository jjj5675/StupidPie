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
            if(DirectorTrigger.instance==null)
              Publisher.Instance.GainOrReleaseControl(true);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(Publisher.Instance.ColliderHasObserver(collision))
        {
            m_TransitioningPresentCount++;
        }

        if(Publisher.Instance.Observers.Count <= m_TransitioningPresentCount && transitionWhen == TransitionWhen.OnTriggerEnter)
        {
            GetComponent<DataSaver>().AutoSaver();
            cellController.CurrentCell.ResetCell(false);
            cellController.SetCell(transitionCell, transitionDestinationTag);
            
            screenManager.autoCameraSetup.SwapVCam(cellController.CurrentCell.confinerCollider);
            Publisher.Instance.GainOrReleaseControl(false);
            m_TransitioningPresent = true;

            TransitionInternal();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (Publisher.Instance.ColliderHasObserver(collision))
        {
            m_TransitioningPresentCount--;
        }
    }

    void TransitionInternal()
    {
        if(transitionType == TransitionType.SameScene)
        {
            Publisher.Instance.SetObservers(false, true, cellController.LastEnteringDestination.locations);
        }
    }
}
