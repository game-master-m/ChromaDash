using System.Collections.Generic;

[System.Serializable]
public class GameSaveData
{
    // PlayerInventoryData에서 가져올 데이터
    public int gold;
    public List<SavedSlotData> mainInventory;
    public SavedSlotData[] quickSlots;

    // PlayerStatsData에서 가져올 데이터
    public int bestScore;

    public GameSaveData()
    {
        gold = 100;
        mainInventory = new List<SavedSlotData>();
        quickSlots = new SavedSlotData[3];
        bestScore = 0;
    }
}