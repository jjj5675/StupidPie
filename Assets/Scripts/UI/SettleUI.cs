using UnityEngine;
using System;
using TMPro;
using UnityEngine.Events;

public class SettleUI : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    [Serializable]
    public class SettleEvent : UnityEvent<SettleUI>
    { }

    public SettleEvent settleEvent;

    protected string[] vs = new string[3];

    private void Awake()
    {
        vs[0] = dialogueText.text;
        vs[1] = scoreText.text;
        vs[2] = timerText.text;
    }

    public void ResetInformation()
    {
        dialogueText.text = vs[0];
        scoreText.text = vs[1];
        timerText.text = vs[2];
    }

    public void Output()
    {
        settleEvent.Invoke(this);

        dialogueText.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(true);
        timerText.gameObject.SetActive(true);
    }
}
