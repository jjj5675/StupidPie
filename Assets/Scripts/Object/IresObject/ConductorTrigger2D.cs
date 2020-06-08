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
        if (collision.CompareTag("Player"))
        {
            if (collision.GetComponent<PlayerBehaviour>().dataBase.abilityTypes.Contains(PlayerDataBase.AbilityType.GIMMICK_ACTIVATE))
            {
                OnEnter.Invoke();
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.GetComponent<PlayerBehaviour>().dataBase.abilityTypes.Contains(PlayerDataBase.AbilityType.GIMMICK_ACTIVATE))
            {
                OnExit.Invoke();
            }
        }
    }
}
