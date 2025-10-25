using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    //Inspector
    [SerializeField] private Transform player;
    [SerializeField] private int initCount = 2;         //초기 segment 생성 갯수
    [SerializeField] private float spawnDistance = 30.0f;   //얘만큼 플레이어와 가까워지면 새로 생성
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

        //1.풀 매니저에 등록
        InitPool();
        //2.초기 전략 설정
        currentStrategy = new EasyStrategy();
        SpawnStartSegment();
    }
    private void Update()
    {
        if (player == null) return;
        //생성... 다음에 TriggerEnter로 감지해서 생성하자
        if (Vector2.Distance(player.position, lastEndPoint.position) < spawnDistance)
        {
            SpawnNextSegment();
            UpdateStrategy();
        }

        //반납... 다음에 TriggerExit 나 TriggerEnter로 감지해서 반납하자...
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
            //time gauge 처리... ㅇㅓ떻게? GameManager에서 관리? 혹은 PlayerController에서 받아와?
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