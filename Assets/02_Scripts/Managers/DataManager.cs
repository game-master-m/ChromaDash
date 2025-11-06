using UnityEngine;
using System.IO;
using System.Text;
using System.Linq;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    [Header("관리할 SSOT 데이터베이스")]
    [SerializeField] private ItemDatabase itemDatabase;

    [Header("관리할 런타임 상태 SO")]
    [SerializeField] private PlayerInventoryData inventoryData;
    [SerializeField] private PlayerStatsData statsData;

    private string _saveFilePath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _saveFilePath = Path.Combine(Application.persistentDataPath, "gameSave.json");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Init()
    {
        InitializeSystems();
        LoadData();
    }

    private void InitializeSystems()
    {
        itemDatabase.Initialize();
        inventoryData.itemDatabase = this.itemDatabase;
    }

    public void SaveData()
    {
        GameSaveData data = new GameSaveData();
        data.gold = inventoryData.Gold;
        data.bestScore = statsData.BestScore;

        data.mainInventory = inventoryData.MainInventory
            .Where(slot => slot != null && slot.itemTemplate != null)
            .Select(slot => new SavedSlotData(slot.itemTemplate.itemName, slot.itemCount))
            .ToList();

        for (int i = 0; i < inventoryData.QuickSlots.Count; i++)
        {
            InventorySlotData slot = inventoryData.QuickSlots[i];
            if (slot != null && slot.itemTemplate != null)
            {
                data.quickSlots[i] = new SavedSlotData(slot.itemTemplate.itemName, slot.itemCount);
            }
            else
            {
                data.quickSlots[i] = null;
            }
        }

        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(_saveFilePath, json, Encoding.UTF8);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[DataManager] 저장 실패: {e.Message}");
        }
    }

    public void LoadData()
    {
        GameSaveData data;

        if (File.Exists(_saveFilePath))
        {
            try
            {
                string json = File.ReadAllText(_saveFilePath, Encoding.UTF8);
                data = JsonUtility.FromJson<GameSaveData>(json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[DataManager] 데이터 로드 실패: {e.Message}. 기본값 사용.");
                data = new GameSaveData();
            }
        }
        else
        {
            data = new GameSaveData();
        }

        statsData.LoadDataFromSave(data);
        inventoryData.LoadDataFromSave(data);

        statsData.Init();
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }
}