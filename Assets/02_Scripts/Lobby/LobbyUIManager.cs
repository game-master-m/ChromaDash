using TMPro;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.UIElements;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] private GameObject mainLobbyPannel;
    [SerializeField] private GameObject shopPannel;
    [SerializeField] private GameObject settingPannel;
    [SerializeField] private GameObject backGround;

    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private PlayerStatsData playerStatsData;

    [SerializeField] private VoidEventChannelSO onPauseRequest;

    [SerializeField] private Transform playerRoot;
    private void Start()
    {
        ShowMainPannel();
    }
    private void OnEnable()
    {
        bestScoreText.text = $"Best Score : {playerStatsData.BestScore}";
        onPauseRequest.OnEvent += ShowMainPannel;
    }
    private void OnDisable()
    {
        onPauseRequest.OnEvent -= ShowMainPannel;
    }
    public void OnClickStartGame()
    {
        if (Managers.Instance != null && Managers.Game != null) Managers.Game.LoadPlayScene();
    }
    public void OnClickQuitGame()
    {
        Application.Quit();
    }
    public void ShowShopPannel()
    {
        Managers.Sound.PlaySFX(ESfxName.BtnShop);
        playerRoot.gameObject.SetActive(false);
        backGround.SetActive(true);
        mainLobbyPannel.SetActive(false);
        shopPannel.SetActive(true);
        settingPannel.SetActive(false);
    }
    public void ShowSettingPannel()
    {
        Managers.Sound.PlaySFX(ESfxName.BtnShop);
        playerRoot.gameObject.SetActive(false);
        backGround.SetActive(true);
        mainLobbyPannel.SetActive(false);
        shopPannel.SetActive(false);
        settingPannel.SetActive(true);
    }
    public void ShowMainPannel()
    {
        mainLobbyPannel.SetActive(true);
        playerRoot.gameObject.SetActive(true);
        backGround.SetActive(false);
        shopPannel.SetActive(false);
        settingPannel.SetActive(false);
    }
}
