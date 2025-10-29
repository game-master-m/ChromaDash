using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemEvent", menuName = "ChromaDash/Events/Item Event Channel")]
public class ItemEventChannelSO : ScriptableObject
{
    public event Action<ItemData> OnEvent;
    public void Raised(ItemData item)
    {
        OnEvent?.Invoke(item);
    }
}
