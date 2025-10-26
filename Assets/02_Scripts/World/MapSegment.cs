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

    //�� �ʵ��� ������ ��(������)
    public List<ExitConnector> exitConnectors;

    //�⺻ �׸���ũ�⿡�� ������ ���� ( ����, �⺻�׸��� ũ�⸦ 2,2�� �Ѵٸ� 2by2 size�� �׸��� ��� �����Ұų� )
    public Vector2Int sizeInCells = new Vector2Int(1, 1);
}
