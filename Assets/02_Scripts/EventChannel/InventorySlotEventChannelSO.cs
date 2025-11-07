
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewIventorySlotEvent", menuName = "ChromaDash/Events/Inventory Slot Event Channel")]
public class InventorySlotEventChannelSO : ScriptableObject
{
    public event Action<InventorySlotData, int> OnEvent;
    public void Raised(InventorySlotData item, int index)
    {
        OnEvent?.Invoke(item, index);
    }
}
