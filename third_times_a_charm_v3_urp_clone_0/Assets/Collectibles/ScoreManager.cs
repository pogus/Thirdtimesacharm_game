using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int score = 0;

    [SerializeField]
    private TextMeshProUGUI scoreText;  // Reference to TextMeshProUGUI component

    void Start()
    {
        if (scoreText == null)
        {
            GameObject scoreCanvas = GameObject.Find("ScoreCanvas");
            if (scoreCanvas != null)
            {
                scoreText = scoreCanvas.transform.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();
            }
        }

        if (scoreText == null)
        {
            Debug.LogError("ScoreText not found! Make sure it exists in the scene under ScoreCanvas.");
        }
        else
        {
            UpdateScoreText();
        }
    }

    public void AddScore(int value)
    {
        score += value;
        UpdateScoreText();
    }

    public int GetScore()
    {
        return score;
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Coins: {score}";
        }
    }
}
