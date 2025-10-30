using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] private GameObject mainLobbyPannel;
    [SerializeField] private GameObject shopPannel;
    [SerializeField] private GameObject inventoryPannel;

    private void Start()
    {
        ShowMainPannel();
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
        mainLobbyPannel.SetActive(false);
        shopPannel.SetActive(true);
        inventoryPannel.SetActive(false);
    }
    public void ShowInventoryPannel()
    {
        mainLobbyPannel.SetActive(false);
        shopPannel.SetActive(false);
        inventoryPannel.SetActive(true);
    }
    public void ShowMainPannel()
    {
        mainLobbyPannel.SetActive(true);
        shopPannel.SetActive(false);
        inventoryPannel.SetActive(false);
    }
}
