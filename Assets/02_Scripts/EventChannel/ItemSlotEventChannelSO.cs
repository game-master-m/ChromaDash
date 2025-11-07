using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemSlotEvent", menuName = "ChromaDash/Events/ItemSlot Event Channel")]
public class ItemSlotEventChannelSO : ScriptableObject
{
    public event Action<ItemData, int> OnEvent;
    public void Raised(ItemData item, int index)
    {
        OnEvent?.Invoke(item, index);
    }
}
