using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private PlayerStatsData playerStatsData;

    [SerializeField] private Transform gameOverPanel;

    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;

    // 현재 스코어도 여기서 처리...
    [SerializeField] private TextMeshProUGUI updatedScoreText;

    private void Awake()
    {
        gameOverPanel.gameObject.SetActive(false);
    }
    private void Start()
    {
        UpdateScore();
    }
    private void OnEnable()
    {
        playerStatsData.onGameOverScoreChange += ShowGameOverUI;
        playerStatsData.onScoreChange += UpdateScore;
    }
    private void OnDisable()
    {
        playerStatsData.onGameOverScoreChange -= ShowGameOverUI;
        playerStatsData.onScoreChange -= UpdateScore;
    }
    private void ShowGameOverUI()
    {
        gameOverPanel.gameObject.SetActive(true);
        currentScoreText.text = $"Score : {playerStatsData.CurrentScore}";
        bestScoreText.text = $"Best : {playerStatsData.BestScore}";
    }
    private void UpdateScore()
    {
        updatedScoreText.text = $"{playerStatsData.CurrentScore}";
    }


}
