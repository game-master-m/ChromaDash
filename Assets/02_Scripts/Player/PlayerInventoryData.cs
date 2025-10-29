using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInventoryData", menuName = "ChromaDash/GameData/PlayerInventoryData")]
public class PlayerInventoryData : ScriptableObject
{
    [Header("재화")]
    [SerializeField] private int gold = 1000;
    public int Gold { get { return gold; } }
    public event Action<int> OnGoldChange;

    [Header("메인 인벤토리")]
    [SerializeField] private List<ItemData> mainInventory = new List<ItemData>();
    public IReadOnlyList<ItemData> MainInventory { get { return mainInventory; } }
    public event Action<ItemData> OnMainInventoryChange;

    [Header("퀵 슬롯")]
    [SerializeField] private ItemData[] quickSlots = new ItemData[3];
    public IReadOnlyList<ItemData> QuickSlots { get { return quickSlots; } }
    public event Action<ItemData, int> OnQuickSlotChange;

    //데이터 수정(매니저로만 호출)
    public bool ModifyGold(int amount)
    {
        if (gold + amount < 0)
        {
            //재화가 부족
            return false;
        }
        gold += amount;
        OnGoldChange?.Invoke(amount);
        return true;
    }

    public void AddItem(ItemData item)
    {
        //인벤 공간 확인? 없으면?
        mainInventory.Add(item);
        OnMainInventoryChange?.Invoke(item);
    }
    public bool RemoveItem(ItemData item)
    {
        bool success = mainInventory.Remove(item);
        //인벤에 제거할 아이템이 없으면 실패.. 실패 메세지?
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
