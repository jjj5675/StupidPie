using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Scoreable : MonoBehaviour
{
    [Serializable]
    public class ScoreEvent : UnityEvent<Scoreable>
    { }

    public ScoreData scoreData;
    public ScoreEvent OnScoreSet;

    private void Start()
    {
        scoreData.newScore = 0;
        scoreData.savedScore = 0;
    }

    public void GainScore(int amount)
    {
        scoreData.newScore += amount;
        OnScoreSet.Invoke(this);
    }

    public void SetScore(int amount)
    {
        scoreData.newScore = amount;
        OnScoreSet.Invoke(this);
    }

    public void SaveScore()
    {
        scoreData.savedScore = scoreData.newScore;
    }
}
