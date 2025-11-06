using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "ChromaDash/GameData/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<ItemData> allItems;

    //itemName을 키로 사용하는 딕셔너리
    private Dictionary<string, ItemData> itemDataDic;

    public void Initialize()
    {
        itemDataDic = new Dictionary<string, ItemData>();
        foreach (ItemData item in allItems)
        {
            if (item == null || string.IsNullOrEmpty(item.itemName))
            {
                continue;
            }

            if (!itemDataDic.ContainsKey(item.itemName))
            {
                itemDataDic.Add(item.itemName, item);
            }
        }
    }

    public ItemData GetItemByName(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;

        itemDataDic.TryGetValue(name, out ItemData item);
        return item;
    }
}