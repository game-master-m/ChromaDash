using System.Collections.Generic;
using UnityEngine;

public class MapSegment : MonoBehaviour
{
    public Transform startPoint;

    //새 맵들이 생성될 곳(여러개)
    public List<Transform> endPoints;

    public ESegmentVericalType EVerticalType = ESegmentVericalType.Any;

    public Transform GetRandomEndPoint()
    {
        if (endPoints == null || endPoints.Count == 0) return transform;
        return endPoints[Random.Range(0, endPoints.Count)];
    }
}
