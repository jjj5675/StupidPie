using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Platform : MonoBehaviour
{
    public enum PlatformType
    {
        NONE, LIGHTING, MOVING, RETURN, OBSTACLE_TRIGGER, FALLING, JUMPING, SWITCH, SPIKE_TRIGGER
    }

    protected enum TriggerState
    {
        ENTER, EXIT
    }

    public bool isMovingAtStart = true;

    protected PlatformType m_PlatformType;
    protected TriggerState m_CurrentTriggerState;
    protected bool m_Started = false;
    protected const int m_SearchObjectCount = 30;

    // Start is called before the first frame update
    void Start()
    {
        Initialise();
    }

    protected abstract void Initialise();

    protected void SearchOverlapPlatforms<TComponent>(Collider2D rootCollider, out TComponent[] copyArray, bool useTrigger, bool addRootCollider = true)
        where TComponent : Platform
    {
        Queue<Collider2D> queue = new Queue<Collider2D>(m_SearchObjectCount);
        List<TComponent> searchList = new List<TComponent>(m_SearchObjectCount);
        Collider2D[] colliderResults = new Collider2D[m_SearchObjectCount];
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.layerMask = LayerMask.NameToLayer("Platform");
        contactFilter.useTriggers = useTrigger;

        queue.Enqueue(rootCollider);

        if (addRootCollider)
        {
            searchList.Add(rootCollider.GetComponent<TComponent>());
        }

        while (queue.Count != 0)
        {
            Collider2D startCollider = queue.Dequeue();
            int count = startCollider.OverlapCollider(contactFilter, colliderResults);

            for (int i = 0; i < count; i++)
            {
                TComponent component = colliderResults[i].GetComponent<TComponent>();

                if (component != null)
                {
                    if (!searchList.Contains(component))
                    {
                        queue.Enqueue(colliderResults[i]);
                        searchList.Add(component);
                    }
                }
            }
        }

        copyArray = searchList.ToArray();
    }

    protected void SearchOverlapObject<TComponent>(Collider2D rootCollider, out TComponent copyObject)
    where TComponent : Component
    {
        Collider2D[] colliderResults = new Collider2D[5];
        ContactFilter2D contactFilter = new ContactFilter2D();

        int count = rootCollider.OverlapCollider(contactFilter, colliderResults);

        for (int i = 0; i < count; i++)
        {
            TComponent component = colliderResults[i].GetComponent<TComponent>();

            if (component != null)
            {
                copyObject = component;
                return;
            }
        }

        copyObject = null;
    }

    public virtual void StartMoving()
    {
        m_Started = true;
    }

    public virtual void StopMoving()
    {
        m_Started = false;
    }
}
