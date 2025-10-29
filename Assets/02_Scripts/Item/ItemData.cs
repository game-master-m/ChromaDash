using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "ChromaDash/GameData/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("����")]
    public string itemName;
    [TextArea]
    public string description;
    public int itemPower;
    public int itemCount;
    public Sprite itemIcon;
    public EItemType eItemType;
    [Header("����")]
    public int buyPrice;
    public int sellPrice;
}
