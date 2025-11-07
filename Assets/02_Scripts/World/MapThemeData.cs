using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMapTheme", menuName = "ChromaDash/MapTheme")]
public class MapThemeData : ScriptableObject
{
    //기본 세그먼트
    public List<MapSegment> easySegments;
    public List<MapSegment> mediumSegments;
    public List<MapSegment> hardSegments;

    //특수 세그먼트
    public List<MapSegment> rhythmEasySegments;
    public List<MapSegment> rhythmMediumSegments;
    public List<MapSegment> rhythmHardSegments;

    public List<MapSegment> coinEasySegments;
    public List<MapSegment> coinMediumSegments;
    public List<MapSegment> coinHardSegments;

    public List<MapSegment> trapEasySegments;
    public List<MapSegment> trapMediumSegments;
    public List<MapSegment> trapHardSegments;

    public HashSet<MapSegment> GetAllUniquePrefabs()
    {
        HashSet<MapSegment> allPrefabs = new HashSet<MapSegment>();

        foreach (MapSegment segment in easySegments) if (segment != null) allPrefabs.Add(segment);
        foreach (MapSegment segment in mediumSegments) if (segment != null) allPrefabs.Add(segment);
        foreach (MapSegment segment in hardSegments) if (segment != null) allPrefabs.Add(segment);

        foreach (MapSegment segment in rhythmEasySegments) if (segment != null) allPrefabs.Add(segment);
        foreach (MapSegment segment in rhythmMediumSegments) if (segment != null) allPrefabs.Add(segment);
        foreach (MapSegment segment in rhythmHardSegments) if (segment != null) allPrefabs.Add(segment);

        foreach (MapSegment segment in coinEasySegments) if (segment != null) allPrefabs.Add(segment);
        foreach (MapSegment segment in coinMediumSegments) if (segment != null) allPrefabs.Add(segment);
        foreach (MapSegment segment in coinHardSegments) if (segment != null) allPrefabs.Add(segment);

        foreach (MapSegment segment in trapEasySegments) if (segment != null) allPrefabs.Add(segment);
        foreach (MapSegment segment in trapMediumSegments) if (segment != null) allPrefabs.Add(segment);
        foreach (MapSegment segment in trapHardSegments) if (segment != null) allPrefabs.Add(segment);

        return allPrefabs;
    }
}
