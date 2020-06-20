using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class InteractOnTrigger2D : MonoBehaviour
{
    public LayerMask layers;
    public int requireEnteringObjectCount;
    public UnityEvent OnEnter, OnExit;

    protected int m_EnteringObjectCount;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!enabled)
        {
            return;
        }

        if(1 << collision.gameObject.layer == layers)
        {
            m_EnteringObjectCount++;

            if (m_EnteringObjectCount == requireEnteringObjectCount)
            {
                OnEnter.Invoke();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!enabled)
        {
            return;
        }

        if (1 << collision.gameObject.layer == layers)
        {
            m_EnteringObjectCount--;
            OnExit.Invoke();
        }
    }
}
