using UnityEngine;
using UnityEngine.UI;

public class EscUI : MonoBehaviour
{
    [Header("�̺�Ʈ ����")]
    [SerializeField] VoidEventChannelSO onPauseRequest;

    [Header("�̺�Ʈ ����")]
    [SerializeField] VoidEventChannelSO onGamePause;
    [SerializeField] VoidEventChannelSO onGameResume;

    [Header("������Ʈ UI")]
    [SerializeField] private Button escButton;
    [SerializeField] private Image escImage;
    [SerializeField] private Transform rootEscPannel;
    [SerializeField] private Button goToLobbyButton;
    [SerializeField] private Button showSettingButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        escButton.onClick.RemoveAllListeners();
        escButton.onClick.AddListener(OnClickEscButton);

        goToLobbyButton.onClick.RemoveAllListeners();
        goToLobbyButton.onClick.AddListener(() => { Managers.Game.LoadLobbyScene(); });

        quitButton.onClick.RemoveAllListeners();
        quitButton.onClick.AddListener(() => { Application.Quit(); });

        rootEscPannel.gameObject.SetActive(false);
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

    private void OnClickEscButton()
    {
        onPauseRequest.Raised();
    }
    private void HandleOnGamePause()
    {
        //�г� �����ֱ�
        rootEscPannel.gameObject.SetActive(true);
        //���� ��ȭ?
        Color escIconColor = escImage.color;
        escImage.color = new Color(escIconColor.r, escIconColor.g, escIconColor.b, 0.5f);
    }
    private void HandleOnGameResume()
    {
        //�г� �ݱ�
        rootEscPannel.gameObject.SetActive(false);
        //���� �������
        Color escIconColor = escImage.color;
        escImage.color = new Color(escIconColor.r, escIconColor.g, escIconColor.b, 1.0f);
    }
}
