using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemPriceText;
    [SerializeField] private Button buyButton;

    private ItemData itemData;
    private Action<ItemData> onBuyCallBack; //Buy��ư Ŭ���� ��ü ����

    public void Init(ItemData itemData, Action<ItemData> onBuyCallBack)
    {
        this.itemData = itemData;
        this.onBuyCallBack = onBuyCallBack;

        itemNameText.text = itemData.itemName;
        itemPriceText.text = $"{itemData.buyPrice} G";

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(OnBuy);
    }

    private void OnBuy()
    {
        onBuyCallBack?.Invoke(itemData);
    }
}
