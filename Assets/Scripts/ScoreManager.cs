using UnityEngine;
using TMPro;
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int score = 0;
    public TMP_Text scoreText;

    private void Awake()
    {
        // Singleton básico
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        UpdateScoreText();
    }

    public void AddPoint(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }
}
