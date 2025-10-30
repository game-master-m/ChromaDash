using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "ChromaDash/GameData/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("����")]
    public string itemName;
    [TextArea]
    public string description;
    public int itemPower;
    public Sprite itemIcon;
    public EItemType eItemType;
    [Header("����")]
    public int buyPrice;
    public int sellPrice;
    [Header("����ŷ ����")]
    public int maxStackCount = 99;
    public int maxQuickSlotStack = 1;

    [HideInInspector]
    public int itemCount = 1;
}
