using UnityEngine;
using System.Collections.Generic;

public class GenerationContext
{
    public uint generatedCount;
    //public float playerTimeGauge;
}
public interface IPathStrategy
{
    MapSegment NextSegment(GenerationContext context, MapThemeData themeData);
}

public abstract class BasePathStrategy : IPathStrategy
{
    public abstract MapSegment NextSegment(GenerationContext context, MapThemeData themeData);

    protected MapSegment GetSegmentFromList(List<MapSegment> segments)
    {
        if (segments == null || segments.Count == 0) return null;
        return segments[Random.Range(0, segments.Count)];
    }
}