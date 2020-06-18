using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    public Text scoreText;
    public ScoreData scoreData;

    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = "X 0";
        scoreData.scoreUI = this;
    }

    public void ChangeScoreText(int count)
    {
        scoreText.text = "X " + count.ToString();
    }
}
