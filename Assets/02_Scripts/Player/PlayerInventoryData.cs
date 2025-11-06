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

    #region Save & Load
    [System.NonSerialized] public ItemDatabase itemDatabase;

    public void LoadDataFromSave(GameSaveData data)
    {
        if (itemDatabase == null)
        {
            return;
        }

        // 1. 골드 복원
        this.gold = data.gold;
        OnGoldChange?.Invoke();

        // 2. 메인 인벤토리 복원 (Hydration)
        mainInventory.Clear();
        foreach (SavedSlotData savedSlot in data.mainInventory)
        {
            ItemData itemTemplate = itemDatabase.GetItemByName(savedSlot.itemName);
            if (itemTemplate != null)
            {
                mainInventory.Add(new InventorySlotData(itemTemplate, savedSlot.itemCount));
            }
        }
        OnMainInventoryChange?.Invoke();

        // 3. 퀵슬롯 복원 (Hydration)
        for (int i = 0; i < quickSlots.Length; i++)
        {
            SavedSlotData savedSlot = data.quickSlots[i];
            if (savedSlot != null && !string.IsNullOrEmpty(savedSlot.itemName))
            {
                ItemData itemTemplate = itemDatabase.GetItemByName(savedSlot.itemName);
                if (itemTemplate != null)
                {
                    quickSlots[i] = new InventorySlotData(itemTemplate, savedSlot.itemCount);
                }
                else
                {
                    quickSlots[i] = null;
                }
            }
            else
            {
                quickSlots[i] = null;
            }
            OnQuickSlotChange?.Invoke(quickSlots[i], i);
        }
    }
    #endregion


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
