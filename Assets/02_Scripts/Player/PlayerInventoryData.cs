using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInventoryData", menuName = "ChromaDash/GameData/PlayerInventoryData")]
public class PlayerInventoryData : ScriptableObject
{
    [Header("데이터")]
    [SerializeField] private int gold = 1000;
    public int Gold { get { return gold; } }
    public event Action OnGoldChange;


    [Header("메인 인벤토리")]
    [SerializeField] private List<InventorySlotData> mainInventory = new List<InventorySlotData>();
    public IReadOnlyList<InventorySlotData> MainInventory { get { return mainInventory; } }
    public event Action OnMainInventoryChange;

    [Header("퀵 슬롯")]
    [SerializeField] private InventorySlotData[] quickSlots = new InventorySlotData[3];
    public IReadOnlyList<InventorySlotData> QuickSlots { get { return quickSlots; } }
    public event Action<InventorySlotData, int> OnQuickSlotChange;

    //데이터 수정(매니저로만 호출)
    public bool ModifyGold(int amount)
    {
        if (gold + amount < 0)
        {
            //재화가 부족
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
    public void AddItemToMainInventory(ItemData itemTemplate, int amountToAdd)
    {
        //인벤 공간 확인? 없으면?

        //스태킹 추가
        if (itemTemplate == null) return;
        bool isStackable = itemTemplate.maxStackCount > 1;
        if (isStackable)
        {
            foreach (InventorySlotData slot in mainInventory)
            {
                if (slot.itemTemplate == itemTemplate && slot.itemCount < itemTemplate.maxStackCount)
                {
                    int remainingSpace = itemTemplate.maxStackCount - slot.itemCount;
                    int amountToMove = Mathf.Min(remainingSpace, amountToAdd);

                    slot.itemCount += amountToMove;
                    amountToAdd -= amountToMove;

                    if (amountToAdd <= 0)
                    {
                        OnMainInventoryChange?.Invoke();
                        return;
                    }
                }
            }

            while (amountToAdd > 0)
            {
                int amountForNewSlot = Mathf.Min(itemTemplate.maxStackCount, amountToAdd);

                mainInventory.Add(new InventorySlotData(itemTemplate, amountForNewSlot));

                amountToAdd -= amountForNewSlot;
            }

            OnMainInventoryChange?.Invoke();
        }
    }
    public bool RemoveItemFromInventory(InventorySlotData slotToRemove)
    {
        bool success = mainInventory.Remove(slotToRemove);
        if (success)
        {
            OnMainInventoryChange?.Invoke();
        }
        return success;
    }
    public void AssignToQuickSlot(InventorySlotData slotData, int index)
    {
        if (index < 0 || index >= quickSlots.Length) return;
        quickSlots[index] = slotData;
        OnQuickSlotChange?.Invoke(slotData, index);
    }
}
