using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopData", menuName = "ChromaDash/GameData/ShopData")]
public class ShopData : ScriptableObject
{
    [SerializeField] List<ItemData> shopItems;

    public List<ItemData> ShopItems { get { return shopItems; } }
}
