using UnityEngine;
using UnityEngine.Events;

public class ScorePickup : MonoBehaviour
{
    public enum PickupState
    {
        NOT_GAIN, GAIN
    }

    public int scoreAmount = 1;
    public ScoreData scoreData;
    public UnityEvent onGivingScore;
    public UnityEvent onEnableScore;

    [HideInInspector]
    public PickupState pickupState;
    int m_PlayerLayerIndex;

    private void Start()
    {
        m_PlayerLayerIndex = LayerMask.NameToLayer("Player");
        pickupState = PickupState.NOT_GAIN;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == m_PlayerLayerIndex)
        {
            pickupState = PickupState.GAIN;
            scoreData.GainScore(scoreAmount);
            onGivingScore.Invoke();
        }
    }
}
