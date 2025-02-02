using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance; // Singleton instance
    private int score = 0;

    [SerializeField]
    private TextMeshProUGUI scoreText; // Reference to TextMeshProUGUI component

    void Awake()
    {
        // Ensure there's only one instance of the ScoreManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (scoreText == null)
        {
            // Search for ScoreText inside ScoreCanvas dynamically
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
