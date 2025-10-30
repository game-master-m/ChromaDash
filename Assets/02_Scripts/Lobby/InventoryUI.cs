using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("������ ����")]
    [SerializeField] private PlayerInventoryData playerData;

    [Header("�̺�Ʈ ����")]
    [SerializeField] private ItemSlotEventChannelSO onEquipToQuickSlotRequest;
    [SerializeField] private ItemEventChannelSO onSellItemRequest;
    [SerializeField] private IntEventChannelSO onUnEquipQuickSlotRequest; //�κ��丮 �Ŵ����� ����

    [Header("UI ������ �� �θ�")]
    [SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField] private Transform mainInventoryContainer;

    [Header("�κ� ������ UI")]
    [SerializeField] private Image[] quickSlotIcons;
    [SerializeField] private TextMeshProUGUI[] quickSlotAmounts;

    private ItemData selectedItemInstance = null;   //���� XXXXXXXXXXXXXXXXXX

    private void OnEnable()
    {
        if (playerData != null)
        {
            playerData.OnMainInventoryChange += RefreshMainInventory;
            playerData.OnQuickSlotChange += RefreshSingleQuickSlot;

            RefreshMainInventory();
            RefreshAllQuickSlots();
        }
    }
    private void OnDisable()
    {
        if (playerData != null)
        {
            playerData.OnMainInventoryChange -= RefreshMainInventory;
            playerData.OnQuickSlotChange -= RefreshSingleQuickSlot;
        }
    }

    private void RefreshMainInventory()
    {
        foreach (Transform child in mainInventoryContainer)
        {
            Destroy(child.gameObject);
        }
        foreach (ItemData itemInstance in playerData.MainInventory)
        {
            GameObject slotGo = Instantiate(inventorySlotPrefab, mainInventoryContainer);
            InventoryItemSlotPrefab slotPrefab = slotGo.GetComponent<InventoryItemSlotPrefab>();
            if (slotPrefab != null)
            {
                slotPrefab.Init(itemInstance, OnSelectItem, OnSellItem);
            }
        }
    }
    private void RefreshSingleQuickSlot(ItemData itemData, int index)
    {
        if (index < 0 || index >= quickSlotIcons.Length) return;
        Image icon = quickSlotIcons[index];
        TextMeshProUGUI slotAmount = quickSlotAmounts[index];
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
    private void RefreshAllQuickSlots()
    {
        for (int i = 0; i < quickSlotIcons.Length; i++)
        {
            ItemData item = playerData.QuickSlots[i];
            RefreshSingleQuickSlot(item, i);
        }
    }
    private void OnSelectItem(ItemData instance)
    {
        selectedItemInstance = instance;
        //ȿ��?
    }
    private void OnSellItem(ItemData instance)
    {
        onSellItemRequest.Raised(instance);
    }
    public void OnClickEquiptToQuickSlot(int index)
    {
        if (selectedItemInstance == null) return;
        onEquipToQuickSlotRequest.Raised(selectedItemInstance, index);
        selectedItemInstance = null;
    }
    public void OnClickUnEquipQuickSlot(int index)
    {
        onUnEquipQuickSlotRequest.Raised(index);
    }
}
