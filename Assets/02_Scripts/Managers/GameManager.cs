using UnityEngine;

public class GameManager : MonoBehaviour
{

    [Header("������ ����")]
    //[SerializeField] private GameSettingsSO gameSettings;
    //[SerializeField] private LevelDataSO levelData;

    [Header("������ �̺�Ʈ")]
    [SerializeField] private VoidEventChannelSO onPauseRequest;
    [SerializeField] private VoidEventChannelSO onPlayerDie;

    [Header("������ �̺�Ʈ")]
    [SerializeField] private VoidEventChannelSO onGamePause;
    [SerializeField] private VoidEventChannelSO onGameResume;
    [SerializeField] private VoidEventChannelSO onGameOver;

    private bool isPause = false;

    private void OnEnable()
    {
        onPlayerDie.OnEvent += HandleGameOver;
        onPauseRequest.OnEvent += TogglePause;
    }
    private void OnDisable()
    {
        onPlayerDie.OnEvent -= HandleGameOver;
        onPauseRequest.OnEvent -= TogglePause;
    }

    public void TogglePause()
    {
        isPause = !isPause;
        if (isPause)
        {
            Time.timeScale = 0.0f;
            onGamePause.Raised();
        }
        else
        {
            Time.timeScale = 1.0f;
            onGameResume.Raised();
        }
    }
    public void HandleGameOver()
    {
        Time.timeScale = 0.5f;
        onGameOver.Raised();
    }
}
