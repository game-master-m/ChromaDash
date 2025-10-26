using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    //Inspector
    [Header("참조")]
    [SerializeField] private Transform player;
    [SerializeField] private MapThemeData currentTheme;

    [Header("PCG 생성 설정")]
    [SerializeField] private MapSegment startSegment;
    [SerializeField] private float spawnDistance = 30.0f;       //얘만큼 플레이어와 가까워지면 새로 생성
    [SerializeField] private int initCount = 5;                //초기 각 segment 생성 갯수

    [Header("겹침 방지 설정")]
    [SerializeField] private Vector2Int gridCellSize = new Vector2Int(2, 1);
    [SerializeField] private int gridCellSpacingY = 4;

    [Header("동적 사이즈 변화")]
    [SerializeField] private Vector2 randomXScaleRange = new Vector2(1.0f, 5.0f);

    [Header("strategy 변경 설정")]
    [SerializeField] private uint modeChangeCountEasyToMedium = 100;

    [Header("맵 Y축 제한")]
    [SerializeField] private float maxHeight = 20.0f;
    [SerializeField] private float minHeight = -6.0f;

    [Header("점프 궤적용")]
    [SerializeField] private Vector2 easyJumpXRange = new Vector2(1f, 3f);
    [SerializeField] private Vector2 mediumJumpXRange = new Vector2(3f, 5f);
    [SerializeField] private Vector2 hardJumpXRange = new Vector2(5f, 7f);
    [SerializeField] private Vector2 chromaHardJumpXRange = new Vector2(7f, 9f);

    //점유 된 그리드 저장용 
    private HashSet<Vector2Int> occupiedGridCells = new HashSet<Vector2Int>();


    //런 타임 변수
    private List<ExitConnector> openConnectors = new List<ExitConnector>();
    private List<MapSegment> activeSegments = new List<MapSegment>();
    private IPathStrategy currentStrategy;
    private GenerationContext context = new GenerationContext();
    private uint generatedCount = 0;

    //캐싱
    private EasyStrategy easyStrategy;
    private MediumStrategy mediumStrategy;
    private RhythmFocusStrategy rhythmFocusStrategy;
    private void Awake()
    {
        easyStrategy = new EasyStrategy();
        mediumStrategy = new MediumStrategy();
        rhythmFocusStrategy = new RhythmFocusStrategy();
    }
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
        currentStrategy = easyStrategy;
        SpawnStartSegment();
    }
    private void InitPool()
    {
        foreach (MapSegment prefab in currentTheme.GetAllUniquePrefabs())
        {

            if (prefab != null) Managers.Pool.CreatePool(prefab, initCount, this.transform);
        }
    }
    private void SpawnStartSegment()
    {
        MapSegment startSegment = GameObject.Instantiate(this.startSegment, Vector3.zero, Quaternion.identity);
        if (startSegment != null)
        {
            startSegment.transform.position = Vector3.zero;
            activeSegments.Add(startSegment);
            generatedCount++;
        }
        //점유 셀 등록
        RegisterOccupiedCells(startSegment, Vector3.zero);

        //다음 맵들이 붙을 곳 저장
        foreach (ExitConnector connector in startSegment.exitConnectors)
        {
            openConnectors.Add(connector);
        }
    }
    private void Update()
    {
        if (player == null) return;

        List<ExitConnector> connectorsToSpawn = new List<ExitConnector>();

        //가까워지면 추가
        foreach (ExitConnector connector in openConnectors)
        {
            if (Vector2.Distance(player.position, connector.jumpStartPoint.position) < spawnDistance)
            {
                connectorsToSpawn.Add(connector);
            }
        }

        //추가 된 출구에서 다음 세그먼트 생성
        foreach (ExitConnector connector in connectorsToSpawn)
        {
            SpawnFromConnector(connector);
        }
        //생성 한 출구들은 목록에서 삭제
        openConnectors.RemoveAll(connectorsToSpawn.Contains);



    }
    public void DespawnSegment(MapSegment segment)
    {
        if (segment == null) return;

        UnregisterOccupiedCells(segment);
        segment.transform.localScale = Vector3.one;
        Managers.Pool.ReturnToPool(segment);
        activeSegments.Remove(segment);
    }
    public void SpawnFromConnector(ExitConnector exitConnector)
    {
        //context Update
        context.generatedCount = this.generatedCount;

        MapSegment prefabToSpawn = currentStrategy.NextSegment(context, currentTheme);

        if (prefabToSpawn == null) return;
        //전 프리팹에서 설정해 놓은 좌표값(출구)에서 점프갭만큼 이동시켜 다음 프리팹 생성
        Vector3 jumpGap = GetJumpGap(exitConnector.eRequiredJump);
        Vector3 newSegmentPos = exitConnector.jumpStartPoint.position + jumpGap;

        //random Scale 적용
        float randomeXScale = Random.Range(randomXScaleRange.x, randomXScaleRange.y);
        Vector3 newScale = new Vector3(randomeXScale, 1f, 1f);

        //높이 제한조건
        if (newSegmentPos.y > maxHeight || newSegmentPos.y < minHeight) return;
        //겹침 방지
        if (CheckForOverlap(prefabToSpawn, newSegmentPos, newScale)) return;
        //새 Segment 생성
        MapSegment newSegment = Managers.Pool.GetFromPool(prefabToSpawn);
        newSegment.transform.position = newSegmentPos;
        newSegment.transform.localScale = newScale;

        activeSegments.Add(newSegment);
        generatedCount++;

        //새 Segment 점유 셀 등록
        RegisterOccupiedCells(newSegment, newSegmentPos);
        //새 Segment의 출구들 등록
        foreach (ExitConnector newConnector in newSegment.exitConnectors)
        {
            openConnectors.Add(newConnector);
        }
    }

    private void RegisterOccupiedCells(MapSegment segment, Vector3 worldPos)
    {
        Vector2Int baseCell = WorldToGridCell(worldPos);
        Vector3 currentScale = segment.transform.localScale;
        int xCells = Mathf.CeilToInt(segment.sizeInCells.x * currentScale.x);
        int yCells = Mathf.CeilToInt(segment.sizeInCells.y * currentScale.y);
        for (int i = 0; i <= xCells; i++)
        {
            for (int j = -gridCellSpacingY / 2; j < yCells + gridCellSpacingY; j++)
            {
                occupiedGridCells.Add(baseCell + new Vector2Int(i, j));
            }
        }
    }
    private void UnregisterOccupiedCells(MapSegment segment)
    {
        Vector2Int baseCell = WorldToGridCell(segment.transform.position);
        Vector3 currentScale = segment.transform.localScale;
        int xCells = Mathf.CeilToInt(currentScale.x * segment.sizeInCells.x);
        int yCells = Mathf.CeilToInt(currentScale.y * segment.sizeInCells.y);
        for (int i = 0; i < xCells; i++)
        {
            for (int j = 0; j < yCells; j++)
            {
                occupiedGridCells.Remove(baseCell + new Vector2Int(i, j));
            }
        }
    }
    private bool CheckForOverlap(MapSegment segment, Vector3 worldPos, Vector3 scaleToApply)
    {
        Vector2Int baseCell = WorldToGridCell(worldPos);
        int xCells = Mathf.CeilToInt(scaleToApply.x * segment.sizeInCells.x);
        int yCells = Mathf.CeilToInt(scaleToApply.y * segment.sizeInCells.y);

        for (int i = 0; i < xCells; i++)
        {
            for (int j = 0; j <= yCells; j++)
            {
                if (occupiedGridCells.Contains(baseCell + new Vector2Int(i, j))) return true;
            }
        }
        return false;
    }
    private Vector2Int WorldToGridCell(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / gridCellSize.x);
        int y = Mathf.FloorToInt(worldPos.y / gridCellSize.y);
        return new Vector2Int(x, y);
    }
    private Vector3 GetJumpGap(EJumpDifficulty eDifficulty)
    {
        Vector2 xRange;
        switch (eDifficulty)
        {
            case EJumpDifficulty.ChromaHard:
                xRange = chromaHardJumpXRange;
                break;
            case EJumpDifficulty.Hard:
                xRange = hardJumpXRange;
                break;
            case EJumpDifficulty.Medium:
                xRange = mediumJumpXRange;
                break;
            default:
                xRange = easyJumpXRange;
                break;
        }
        float radius = (xRange.y - xRange.x) / 2;
        Vector2 randomInCircle = Random.insideUnitCircle * radius;

        return new Vector3(randomInCircle.x + xRange.x, randomInCircle.y, 0);
    }


    //테스트용..
    private void UpdateStrategy()
    {
        if (generatedCount > modeChangeCountEasyToMedium && currentStrategy is EasyStrategy)
        {
            currentStrategy = mediumStrategy;
        }
    }
}