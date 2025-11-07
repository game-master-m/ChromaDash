
[System.Serializable]
public class InventorySlotData
{
    public ItemData itemTemplate;

    public int itemCount;

    public InventorySlotData(ItemData template, int count)
    {
        itemTemplate = template;
        itemCount = count;
    }
}
