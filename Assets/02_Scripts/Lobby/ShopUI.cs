using TMPro;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [Header("데이터 참조(SO)")]
    [SerializeField] private ShopData shopData;
    [SerializeField] private PlayerInventoryData playerData;

    [Header("이벤트 발행")]
    [SerializeField] ItemEventChannelSO onBuyItemRequest; //InventoryManager가 구독

    [Header("UI 프리팹 및 부모")]
    [SerializeField] private GameObject shopItemSlotPrefab;
    [SerializeField] private Transform slotContainer;
    [SerializeField] private TextMeshProUGUI playerGoldText;

    private void OnEnable()
    {
        RefreshShopUI();
        if (playerData != null)
        {
            playerData.OnGoldChange += UpdateGoldUI;
            UpdateGoldUI();
        }
    }
    private void OnDisable()
    {
        if (playerData != null) playerData.OnGoldChange -= UpdateGoldUI;
    }
    public void UpdateGoldUI()
    {
        if (playerGoldText != null) playerGoldText.text = $"Gold : {playerData.Gold}";
    }
    public void RefreshShopUI()
    {
        //기존 슬롯들 삭제
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }
        //ShopData의 아이템 목록을 기반으로 슬롯 생성
        if (shopData == null) return;
        foreach (ItemData itemData in shopData.ShopItems)
        {
            GameObject slotGo = Instantiate(shopItemSlotPrefab, slotContainer);
            ShopItemSlotUI slotUI = slotGo.GetComponent<ShopItemSlotUI>();
            if (slotUI != null)
            {
                slotUI.Init(itemData, OnBuyButtonClicked);
            }
        }
    }
    private void OnBuyButtonClicked(ItemData itemData)
    {
        if (playerData.Gold < itemData.buyPrice)
        {
            //골드부족 메세지
            return;
        }
        if (onBuyItemRequest != null)
        {
            //구매성공~ 이후 아이템 처리? 어케하지?
            onBuyItemRequest.Raised(itemData);  //InventoryManager가 구독
        }
    }
}
