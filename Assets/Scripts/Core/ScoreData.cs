using UnityEngine;

[CreateAssetMenu(fileName = "ScoreData", menuName = "Data/ScoreData")]
public class ScoreData : ScriptableObject
{
    [HideInInspector]
    public int newScore = 0;
    public int savedScore;
}
