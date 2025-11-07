using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemSlotPrefab : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemPriceText;
    [SerializeField] private Button buyButton;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;

    private ItemData itemData;
    private Action<ItemData> onBuyCallBack; //Buy버튼 클릭시 자체 발행

    public void Init(ItemData itemData, Action<ItemData> onBuyCallBack)
    {
        this.itemData = itemData;
        this.itemIcon.sprite = itemData.itemIcon;
        this.itemDescriptionText.text = itemData.description;
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
