using UnityEngine;
using UnityEngine.UI;

public class ScoreView : MonoBehaviour
{
    private static string SCORE_FORMAT = "Score: {0}";
    public Text TextComponent;
    private int Score;

    public void IncrementScore(int byValue)
    {
        Score += byValue;
        UpdateScoreText();
    }

	private void Start () {
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        TextComponent.text = string.Format(SCORE_FORMAT, Score);
    }
}
