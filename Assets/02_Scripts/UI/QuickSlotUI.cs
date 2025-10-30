using System.Runtime.ExceptionServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class QuickSlotUI : MonoBehaviour
{
    [Header("데이터 참조")]
    [SerializeField] private PlayerInventoryData playerData;

    [Header("UI 컴포넌트")]
    [SerializeField] private Image[] slotIcons;
    [SerializeField] private TextMeshProUGUI[] slotAmounts;

    private void OnEnable()
    {
        playerData.OnQuickSlotChange += UpdateQuickSlotUI;
    }
    private void OnDisable()
    {
        playerData.OnQuickSlotChange -= UpdateQuickSlotUI;
    }

    private void UpdateQuickSlotUI(ItemData itemData, int index)
    {
        if (index < 0 || index >= slotIcons.Length) return;
        Image icon = slotIcons[index];
        TextMeshProUGUI slotAmount = slotAmounts[index];
        if (itemData != null)
        {
            icon.sprite = itemData.itemIcon;
            icon.color = Color.white;
            slotAmount.text = $"{itemData.itemCount}";
        }
        else
        {
            icon.sprite = null;
            icon.color = new Color(1, 1, 1, 0.0f);
            slotAmount.text = "";
        }
    }
}
