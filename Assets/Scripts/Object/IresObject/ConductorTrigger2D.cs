using UnityEngine;
using UnityEngine.Events;

public class ConductorTrigger2D : MonoBehaviour
{
    public UnityEvent OnEnter;
    public UnityEvent OnExit;

    Collider2D m_Collider;

    // Start is called before the first frame update
    void Reset()
    {
        m_Collider = GetComponent<Collider2D>();
        m_Collider.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(PlayableCharacterFactory.TryGetCollider(PlayerBehaviour.PlayableCharacter.IRES) == collision)
        {
            OnEnter.Invoke();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (PlayableCharacterFactory.TryGetCollider(PlayerBehaviour.PlayableCharacter.IRES) == collision)
        {
            OnExit.Invoke();
        }
    }
}
