using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMapTheme", menuName = "ChromaDash/MapTheme")]
public class MapThemeData : ScriptableObject
{
    public List<MapSegment> easySegments;
    public List<MapSegment> mediumSegments;
    public List<MapSegment> hardSegments;
    public List<MapSegment> rhythmSegments;

    public HashSet<MapSegment> GetAllUniquePrefabs()
    {
        HashSet<MapSegment> allPrefabs = new HashSet<MapSegment>();

        foreach (MapSegment segment in easySegments) if (segment != null) allPrefabs.Add(segment);
        foreach (MapSegment segment in mediumSegments) if (segment != null) allPrefabs.Add(segment);
        foreach (MapSegment segment in hardSegments) if (segment != null) allPrefabs.Add(segment);
        foreach (MapSegment segment in rhythmSegments) if (segment != null) allPrefabs.Add(segment);

        return allPrefabs;
    }
}
