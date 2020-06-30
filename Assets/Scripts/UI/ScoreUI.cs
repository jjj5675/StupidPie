using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = "X 0";
    }

    public void ChangeScoreText(Scoreable scoreable)
    {
        scoreText.text = "X " + scoreable.scoreData.newScore.ToString();
    }

    public void TakeScore(SettleUI settle)
    {
        settle.scoreText.text += scoreText.text.Substring(2, 1);
    }
}
