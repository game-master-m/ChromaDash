using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "ChromaDash/GameData/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("정보")]
    public string itemName;
    [TextArea]
    public string description;
    public int itemPower;
    public int itemCount;
    public Sprite itemIcon;
    public EItemType eItemType;
    [Header("가격")]
    public int buyPrice;
    public int sellPrice;
}
