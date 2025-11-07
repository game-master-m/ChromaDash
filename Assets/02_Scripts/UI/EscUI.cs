using UnityEngine;
using UnityEngine.UI;

public class EscUI : MonoBehaviour
{
    [Header("이벤트 발행")]
    [SerializeField] VoidEventChannelSO onPauseRequest;

    [Header("이벤트 구독")]
    [SerializeField] VoidEventChannelSO onGamePause;
    [SerializeField] VoidEventChannelSO onGameResume;

    [Header("컴포넌트 UI")]
    [SerializeField] private Button escButton;
    [SerializeField] private Image escImage;
    [SerializeField] private Transform rootEscPannel;
    [SerializeField] private Button goToLobbyButton;
    [SerializeField] private Button showSettingButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button retryButton;

    [SerializeField] private GameObject settingPannel;
    private void Awake()
    {
        escButton.onClick.RemoveAllListeners();
        escButton.onClick.AddListener(OnClickEscButton);

        goToLobbyButton.onClick.RemoveAllListeners();
        goToLobbyButton.onClick.AddListener(() => { Managers.Game.LoadLobbyScene(); });

        quitButton.onClick.RemoveAllListeners();
        quitButton.onClick.AddListener(() => { Application.Quit(); });

        retryButton.onClick.RemoveAllListeners();
        retryButton.onClick.AddListener(OnClickRetryButton);

        showSettingButton.onClick.RemoveAllListeners();
        showSettingButton.onClick.AddListener(() =>
        {
            settingPannel.SetActive(true);
            rootEscPannel.gameObject.SetActive(false);
        });


        rootEscPannel.gameObject.SetActive(false);
    }
    public void OnClickReturnEscPannel()
    {
        rootEscPannel.gameObject.SetActive(true);
        settingPannel.SetActive(false);
    }
    private void OnEnable()
    {
        onGamePause.OnEvent += HandleOnGamePause;
        onGameResume.OnEvent += HandleOnGameResume;
    }
    private void OnDisable()
    {
        onGamePause.OnEvent -= HandleOnGamePause;
        onGameResume.OnEvent -= HandleOnGameResume;
    }
    private void OnClickRetryButton()
    {
        Managers.Game.LoadPlayScene();
    }
    private void OnClickEscButton()
    {
        onPauseRequest.Raised();
    }
    private void HandleOnGamePause()
    {
        //패널 보여주기
        rootEscPannel.gameObject.SetActive(true);
        settingPannel.SetActive(false);
        //색상 변화?
        Color escIconColor = escImage.color;
        escImage.color = new Color(escIconColor.r, escIconColor.g, escIconColor.b, 0.5f);
    }
    private void HandleOnGameResume()
    {
        //패널 닫기
        rootEscPannel.gameObject.SetActive(false);
        settingPannel.SetActive(false);
        //색상 원래대로
        Color escIconColor = escImage.color;
        escImage.color = new Color(escIconColor.r, escIconColor.g, escIconColor.b, 1.0f);
    }
}
