
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewIventoryEvent", menuName = "ChromaDash/Events/Inventory Event Channel")]
public class InventoryEventChannelSO : ScriptableObject
{
    public event Action<InventorySlotData> OnEvent;
    public void Raised(InventorySlotData item)
    {
        OnEvent?.Invoke(item);
    }
}
