using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "ChromaDash/GameData/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("정보")]
    public string itemName;
    [TextArea]
    public string description;
    public int itemPower;
    public Sprite itemIcon;
    public EItemType eItemType;
    [Header("가격")]
    public int buyPrice;
    public int sellPrice;
    [Header("스태킹 설정")]
    public int maxStackCount = 99;
    public int maxQuickSlotStack = 1;

    [HideInInspector]
    public int itemCount = 1;
}
