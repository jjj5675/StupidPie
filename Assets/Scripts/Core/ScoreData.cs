using UnityEngine;

[CreateAssetMenu(fileName = "ScoreData", menuName = "Data/ScoreData")]
public class ScoreData : ScriptableObject
{
    [HideInInspector]
    public ScoreUI scoreUI;
    public int newScore = 0;
    public int savedScore;


    public void GainScore(int amount)
    {
        newScore += amount;
        scoreUI.ChangeScoreText(newScore);
    }

    public void SaveData()
    {
        savedScore = newScore;
        scoreUI.ChangeScoreText(savedScore);
    }

    public void ResetData()
    {
        newScore = savedScore;
        scoreUI.ChangeScoreText(newScore);
    }
}
