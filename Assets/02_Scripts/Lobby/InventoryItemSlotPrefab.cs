using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemSlotPrefab : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemCountText;
    [SerializeField] private Button selectButton;
    [SerializeField] private Button sellButton;

    private ItemData itemInstance; //¿øº»XXXXXXXXXXXXXXXXX 
    private Action<ItemData> onSelectCallBack;
    private Action<ItemData> onSellCallBack;

    public void Init(ItemData instance, Action<ItemData> onSelect, Action<ItemData> onSell)
    {
        itemInstance = instance;
        onSelectCallBack = onSelect;
        onSellCallBack = onSell;

        itemIcon.sprite = itemInstance.itemIcon;
        itemIcon.color = Color.white;
        itemNameText.text = itemInstance.name;
        itemCountText.text = $"{itemInstance.itemCount}";

        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnSelect);
        }
        if (sellButton != null)
        {
            sellButton.onClick.RemoveAllListeners();
            sellButton.onClick.AddListener(OnSell);
        }
    }
    private void OnSelect()
    {
        onSelectCallBack?.Invoke(itemInstance);
    }
    private void OnSell()
    {
        onSellCallBack?.Invoke(itemInstance);
    }
}
