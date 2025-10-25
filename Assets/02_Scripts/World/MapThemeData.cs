using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMapTheme", menuName = "ChromaDash/MapTheme")]
public class MapThemeData : ScriptableObject
{
    public MapSegment startSegment;

    public List<MapSegment> easySegments;
    public List<MapSegment> mediumSegments;
    public List<MapSegment> hardSegments;
    public List<MapSegment> rhythmSegments;

    public HashSet<MapSegment> GetAllMapSegments()
    {
        HashSet<MapSegment> allSegments = new HashSet<MapSegment>();

        if (startSegment != null) allSegments.Add(startSegment);

        foreach (MapSegment segment in easySegments) if (segment != null) allSegments.Add(segment);
        foreach (MapSegment segment in mediumSegments) if (segment != null) allSegments.Add(segment);
        foreach (MapSegment segment in hardSegments) if (segment != null) allSegments.Add(segment);
        foreach (MapSegment segment in rhythmSegments) if (segment != null) allSegments.Add(segment);

        return allSegments;
    }
}
