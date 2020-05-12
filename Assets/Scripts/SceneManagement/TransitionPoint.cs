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

    public GameObject transitioningSeri;
    public GameObject transitioningIres;
    public TransitionType transitionType;
    public Cell transitionCell;
    public CellTransitionDestination.DestinationTag transitionDestinationTag;
    public TransitionWhen transitionWhen;
    public bool resetInputValueOnTransition = true;

    bool m_TransitioningSeriPresent = false;
    bool m_TransitioningIresPresent = false;


    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == transitioningSeri)
        {
            m_TransitioningSeriPresent = true;
        }
        else if(collision.gameObject == transitioningIres)
        {
            m_TransitioningIresPresent = true;
        }

        if(m_TransitioningIresPresent && m_TransitioningSeriPresent && transitionWhen == TransitionWhen.OnTriggerEnter)
        {
            CellController.Instance.SetCells(transitionCell);
            TransitionInternal();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == transitioningSeri)
        {
            m_TransitioningSeriPresent = false;
        }
        else if (collision.gameObject == transitioningIres)
        {
            m_TransitioningIresPresent = false;
        }
    }

    void TransitionInternal()
    {
        if(transitionType == TransitionType.SameScene)
        {
            GameObjectTeleporter.Teleport(transitioningSeri, transitionDestinationTag);
            GameObjectTeleporter.Teleport(transitioningIres, transitionDestinationTag);
        }
    }

}
