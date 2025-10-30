using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInventoryData", menuName = "ChromaDash/GameData/PlayerInventoryData")]
public class PlayerInventoryData : ScriptableObject
{
    [Header("������")]
    [SerializeField] private int gold = 1000;
    public int Gold { get { return gold; } }
    public event Action OnGoldChange;


    [Header("���� �κ��丮")]
    [SerializeField] private List<ItemData> mainInventory = new List<ItemData>();
    public IReadOnlyList<ItemData> MainInventory { get { return mainInventory; } }
    public event Action OnMainInventoryChange;

    [Header("�� ����")]
    [SerializeField] private ItemData[] quickSlots = new ItemData[3];
    public IReadOnlyList<ItemData> QuickSlots { get { return quickSlots; } }
    public event Action<ItemData, int> OnQuickSlotChange;

    //������ ����(�Ŵ����θ� ȣ��)
    public bool ModifyGold(int amount)
    {
        if (gold + amount < 0)
        {
            //��ȭ�� ����
            return false;
        }
        gold += amount;
        OnGoldChange?.Invoke();
        return true;
    }
    public void NotifyMainInventoryChange()
    {
        OnMainInventoryChange?.Invoke();
    }
    public void AddItemToMainInventory(ItemData itemInstance)
    {
        //�κ� ���� Ȯ��? ������?

        //����ŷ �߰�
        ItemData stackableItem = MainInventory.FirstOrDefault(
            i => i.itemName == itemInstance.itemName && i.itemCount < i.maxStackCount);
        if (stackableItem != null)
        {
            int remainingSpace = stackableItem.maxStackCount - stackableItem.itemCount;
            int amountToAdd = Mathf.Min(remainingSpace, itemInstance.itemCount);

            stackableItem.itemCount += amountToAdd;
            itemInstance.itemCount -= amountToAdd;
        }
        if (itemInstance.itemCount > 0)
        {
            mainInventory.Add(itemInstance);
        }
        else
        {
            Destroy(itemInstance);
        }
        OnMainInventoryChange?.Invoke();
    }
    public bool RemoveItemFromInventory(ItemData itemInstance)
    {
        bool success = mainInventory.Remove(itemInstance);
        if (success)
        {
            OnMainInventoryChange?.Invoke();
        }
        return success;
    }
    public void AssignToQuickSlot(ItemData itemInstance, int index)
    {
        if (index < 0 || index >= quickSlots.Length) return;
        quickSlots[index] = itemInstance;
        OnQuickSlotChange?.Invoke(itemInstance, index);
    }
}
