using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExitConnector
{
    public Transform jumpStartPoint;
    public EJumpDifficulty eRequiredJump;
}

public class MapSegment : MonoBehaviour
{
    public Transform startPoint;

    //동적 사이즈 적용 유무
    public bool isDynamicSize;
    //새 맵들이 생성될 곳(여러개)
    public List<ExitConnector> exitConnectors;

    //기본 그리드크기에서 차지할 영역 ( 만약, 기본그리드 크기를 2,2를 한다면 2by2 size의 그리드 몇개를 점유할거냐 )
    public Vector2Int sizeInCells = new Vector2Int(1, 1);
}
