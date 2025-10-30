using TMPro;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [Header("������ ����(SO)")]
    [SerializeField] private ShopData shopData;
    [SerializeField] private PlayerInventoryData playerData;

    [Header("�̺�Ʈ ����")]
    [SerializeField] ItemEventChannelSO onBuyItemRequest; //InventoryManager�� ����

    [Header("UI ������ �� �θ�")]
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
        //���� ���Ե� ����
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }
        //ShopData�� ������ ����� ������� ���� ����
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
            //������ �޼���
            return;
        }
        if (onBuyItemRequest != null)
        {
            //���ż���~ ���� ������ ó��? ��������?
            onBuyItemRequest.Raised(itemData);  //InventoryManager�� ����
        }
    }
}
