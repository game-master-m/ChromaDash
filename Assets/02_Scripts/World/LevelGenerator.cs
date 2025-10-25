using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    //Inspector
    [SerializeField] private Transform player;
    [SerializeField] private int initCount = 2;         //�ʱ� segment ���� ����
    [SerializeField] private float spawnDistance = 30.0f;   //�길ŭ �÷��̾�� ��������� ���� ����
    [SerializeField] private float deSpawnDistance = 15.0f;
    [SerializeField] private MapThemeData currentTheme;
    [SerializeField] private float maxHeight = 20.0f;
    [SerializeField] private float minHeight = -3.0f;

    [SerializeField] private uint modeChangeCountEasyToMedium = 100;

    private Queue<MapSegment> activeSegmentQue = new Queue<MapSegment>();
    private Transform lastEndPoint;
    private IPathStrategy currentStrategy;
    private uint generatedCount = 0;

    private void Start()
    {
        if (player == null || currentTheme == null)
        {
            this.enabled = false;
            return;
        }

        //1.Ǯ �Ŵ����� ���
        InitPool();
        //2.�ʱ� ���� ����
        currentStrategy = new EasyStrategy();
        SpawnStartSegment();
    }
    private void Update()
    {
        if (player == null) return;
        //����... ������ TriggerEnter�� �����ؼ� ��������
        if (Vector2.Distance(player.position, lastEndPoint.position) < spawnDistance)
        {
            SpawnNextSegment();
            UpdateStrategy();
        }

        //�ݳ�... ������ TriggerExit �� TriggerEnter�� �����ؼ� �ݳ�����...
        if (activeSegmentQue.Count > initCount)
        {
            MapSegment oldestSegment = activeSegmentQue.Peek();
            if (oldestSegment == null)
            {
                activeSegmentQue.Dequeue();
                return;
            }
            if (Vector2.Distance(player.position, oldestSegment.GetRandomEndPoint().position) > deSpawnDistance)
            {
                DespawnOldestSegment();
            }
        }
    }

    private void InitPool()
    {
        foreach (MapSegment segment in currentTheme.GetAllMapSegments())
        {
            Managers.Pool.CreatePool(segment, initCount, this.transform);
        }
    }
    private void SpawnStartSegment()
    {
        MapSegment startSegment = Managers.Pool.GetFromPool(currentTheme.startSegment);
        if (startSegment != null)
        {
            startSegment.transform.position = Vector3.zero;
            activeSegmentQue.Enqueue(startSegment);
            lastEndPoint = startSegment.GetRandomEndPoint();
            generatedCount++;
        }
    }

    public void SpawnNextSegment()
    {
        GenerationContext context = new GenerationContext
        {
            currentHeight = lastEndPoint.position.y,
            maxHeight = this.maxHeight,
            minHeight = this.minHeight,
            generatedCount = this.generatedCount,
            //time gauge ó��... ���ö���? GameManager���� ����? Ȥ�� PlayerController���� �޾ƿ�?
        };

        MapSegment nextPrefab = currentStrategy.NextSegment(context, currentTheme);

        if (nextPrefab == null)
        {
            nextPrefab = currentTheme.easySegments[Random.Range(0, currentTheme.easySegments.Count)];
        }

        MapSegment nextSegment = Managers.Pool.GetFromPool(nextPrefab);

        Transform entry = nextSegment.startPoint;
        Transform exit = lastEndPoint;

        Vector3 newPosition = exit.position + nextSegment.transform.position - entry.position;
        nextSegment.transform.position = newPosition;

        activeSegmentQue.Enqueue(nextSegment);
        lastEndPoint = nextSegment.GetRandomEndPoint();
        generatedCount++;
    }

    public void DespawnOldestSegment()
    {
        if (activeSegmentQue.Count == 0) return;
        Managers.Pool.ReturnToPool(activeSegmentQue.Dequeue());
    }

    private void UpdateStrategy()
    {
        if (generatedCount > modeChangeCountEasyToMedium && currentStrategy is EasyStrategy)
        {
            currentStrategy = new MediumStrategy();
        }
    }
}