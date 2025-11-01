using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private readonly string playSceneName = "PlayScene";
    private readonly string lobbySceneName = "LobbyScene";

    [Header("������ ����")]
    //[SerializeField] private GameSettingsSO gameSettings;
    //[SerializeField] private LevelDataSO levelData;

    [Header("�̺�Ʈ ����")]
    [SerializeField] private VoidEventChannelSO onPauseRequest;
    [SerializeField] private VoidEventChannelSO onPlayerDie;

    [Header("�̺�Ʈ ����")]
    [SerializeField] private VoidEventChannelSO onGameStart;
    [SerializeField] private VoidEventChannelSO onGamePause;
    [SerializeField] private VoidEventChannelSO onGameResume;
    [SerializeField] private VoidEventChannelSO onGameOver;


    private bool isPause = false;

    private void Start()
    {
        //LoadPlayScene();
        LoadLobbyScene();
    }
    private void OnEnable()
    {
        onPlayerDie.OnEvent += HandleGameOver;
        onPauseRequest.OnEvent += TogglePause;

        //�� ��ȯ����
        SceneManager.sceneLoaded += HandleOnSceneLoad;
    }
    private void OnDisable()
    {
        onPlayerDie.OnEvent -= HandleGameOver;
        onPauseRequest.OnEvent -= TogglePause;
        SceneManager.sceneLoaded -= HandleOnSceneLoad;
    }
    public void HandleOnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == playSceneName)
        {
            if (onGameStart != null) onGameStart.Raised();
        }
    }
    public void LoadPlayScene()
    {
        Time.timeScale = 1.0f;
        isPause = false;
        SceneManager.LoadScene(playSceneName);
    }
    public void LoadLobbyScene()
    {
        Time.timeScale = 1.0f;
        isPause = false;
        SceneManager.LoadScene(lobbySceneName);
    }
    public void TogglePause()
    {
        if (SceneManager.GetActiveScene().name != "PlayScene") return;
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
