using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public ScoreData scoreData;

    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = "X 0";
        scoreData.scoreUI = this;
        scoreData.newScore = 0;
        scoreData.savedScore = 0;
    }

    public void ChangeScoreText(int count)
    {
        scoreText.text = "X " + count.ToString();
    }

    public void TakeScore(SettleUI settle)
    {
        settle.scoreText.text += scoreText.text.Substring(2, 1);
    }
}
