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

    // Start is called before the first frame update
    void Start()
    {
        Initialise();
    }

    public abstract void ResetPlatform();
    protected abstract void Initialise();

    protected void SearchOverlapPlatforms<TComponent>(Collider2D rootCollider, out TComponent[] copyArray, int resultCount, bool addRootCollider = true)
        where TComponent : Platform
    {
        Queue<Collider2D> queue = new Queue<Collider2D>(resultCount);
        List<TComponent> searchList = new List<TComponent>(resultCount);
        Collider2D[] colliderResults = new Collider2D[resultCount];
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = true;

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

    protected void SearchOverlapCharacter(Collider2D collider, ContactFilter2D contactFilter2D, int ressultCount)
    {
        collider.enabled = true;
        Collider2D[] colliderResults = new Collider2D[ressultCount];
        PlayerBehaviour playerBehaviour;

        int count = collider.OverlapCollider(contactFilter2D, colliderResults);

        for (int i = 0; i < count; i++)
        {
            if(colliderResults[i] == PlayableCharacterFactory.TryGetCollider(PlayerBehaviour.PlayableCharacter.IRES))
            {
                playerBehaviour = PlayableCharacterFactory.TryGetBehaviour(PlayerBehaviour.PlayableCharacter.IRES);
                playerBehaviour.damageable.TakeDamage(null, true);
                break;
            }
            else if (colliderResults[i] == PlayableCharacterFactory.TryGetCollider(PlayerBehaviour.PlayableCharacter.SERI))
            {
                playerBehaviour = PlayableCharacterFactory.TryGetBehaviour(PlayerBehaviour.PlayableCharacter.SERI);
                playerBehaviour.damageable.TakeDamage(null, true);
                break;
            }
        }

        collider.enabled = false;
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
