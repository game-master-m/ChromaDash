using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInventoryData", menuName = "ChromaDash/GameData/PlayerInventoryData")]
public class PlayerInventoryData : ScriptableObject
{
    [Header("��ȭ")]
    [SerializeField] private int gold = 1000;
    public int Gold { get { return gold; } }
    public event Action<int> OnGoldChange;

    [Header("���� �κ��丮")]
    [SerializeField] private List<ItemData> mainInventory = new List<ItemData>();
    public IReadOnlyList<ItemData> MainInventory { get { return mainInventory; } }
    public event Action<ItemData> OnMainInventoryChange;

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
        OnGoldChange?.Invoke(amount);
        return true;
    }

    public void AddItem(ItemData item)
    {
        //�κ� ���� Ȯ��? ������?
        mainInventory.Add(item);
        OnMainInventoryChange?.Invoke(item);
    }
    public bool RemoveItem(ItemData item)
    {
        bool success = mainInventory.Remove(item);
        //�κ��� ������ �������� ������ ����.. ���� �޼���?
        if (success)
        {
            OnMainInventoryChange?.Invoke(item);
        }
        return success;
    }
    public void AssignToQuickSlot(ItemData item, int index)
    {
        if (index < 0 || index >= quickSlots.Length) return;
        quickSlots[index] = item;
        OnQuickSlotChange?.Invoke(item, index);
    }
}
