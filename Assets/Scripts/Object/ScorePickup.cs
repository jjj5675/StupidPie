using UnityEngine;
using UnityEngine.Events;

public class ScorePickup : MonoBehaviour
{
    public enum PickupState
    {
        NOT_GAIN, GAIN
    }

    public int scoreAmount = 1;
    public UnityEvent onGivingScore;
    public UnityEvent onEnableScore;

    [HideInInspector]
    public PickupState pickupState;

    private void Start()
    {
        pickupState = PickupState.NOT_GAIN;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(Publisher.Instance.TryGetObserver(collision, out Observer observer))
        {
            Debug.Log("!!");
            pickupState = PickupState.GAIN;
            
            observer.PlayerInfo.scoreable.GainScore(scoreAmount);
            onGivingScore.Invoke();
        }
    }
}
