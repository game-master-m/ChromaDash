using UnityEngine;

public class Managers : MonoBehaviour
{
    public static Managers Instance { get; private set; }
    [Header("�Ŵ��� ������")]
    [SerializeField] private GameObject poolManagerPrefab;
    [SerializeField] private GameObject inputManagerPrefab;
    [SerializeField] private GameObject gameManagerPrefab;
    [SerializeField] private GameObject inventoryManagerPrefab;
    [SerializeField] private GameObject playerStatsManagerPrefab;

    public static PoolManager Pool { get; private set; }
    public static InputManager Input { get; private set; }
    public static GameManager Game { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        //�Ŵ����� ����
        if (poolManagerPrefab != null)
        {
            GameObject poolGo = Instantiate(poolManagerPrefab, transform);
            Pool = poolGo.GetComponent<PoolManager>();
        }
        if (inputManagerPrefab != null)
        {
            GameObject inputGo = Instantiate(inputManagerPrefab, transform);
            Input = inputGo.GetComponent<InputManager>();
        }

        if (gameManagerPrefab != null)
        {
            GameObject gameGo = Instantiate(gameManagerPrefab, transform);
            Game = gameGo.GetComponent<GameManager>();
        }
        if (inventoryManagerPrefab != null)
        {
            GameObject invenGo = Instantiate(inventoryManagerPrefab, transform);
        }
        if (playerStatsManagerPrefab != null)
        {
            GameObject statsGo = Instantiate(playerStatsManagerPrefab, transform);
        }

    }

}