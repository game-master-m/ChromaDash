[System.Serializable]
public class SavedSlotData
{
    public string itemName;
    public int itemCount;

    public SavedSlotData() { }

    public SavedSlotData(string name, int count)
    {
        itemName = name;
        itemCount = count;
    }
}