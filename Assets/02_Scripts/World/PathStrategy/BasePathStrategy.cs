
using UnityEngine;
using System.Collections.Generic;

public class GenerationContext
{
    public float currentHeight;
    public float maxHeight;
    public float minHeight;
    public float playerTimeGauge;
    public uint generatedCount;
}
public interface IPathStrategy
{
    MapSegment NextSegment(GenerationContext context, MapThemeData themeData);
}

public abstract class BasePathStrategy : IPathStrategy
{
    public abstract MapSegment NextSegment(GenerationContext context, MapThemeData themeData);

    protected MapSegment GetSegmentFromList(GenerationContext context, List<MapSegment> segments)
    {
        if (segments == null || segments.Count == 0) return null;
        List<MapSegment> result = new List<MapSegment>();

        if (context.currentHeight >= context.maxHeight)
        {
            result.AddRange(segments.FindAll(s => s.
            EVerticalType == ESegmentVericalType.Middle ||
            s.EVerticalType == ESegmentVericalType.Low ||
            s.EVerticalType == ESegmentVericalType.Any));
        }
        else if (context.currentHeight <= context.minHeight)
        {
            result.AddRange(segments.FindAll(s =>
            s.EVerticalType == ESegmentVericalType.Middle ||
            s.EVerticalType == ESegmentVericalType.High ||
            s.EVerticalType == ESegmentVericalType.Any));
        }
        else
        {
            result.AddRange(segments);
        }

        if (result.Count == 0)
        {
            return segments[Random.Range(0, segments.Count)];
        }
        return result[Random.Range(0, result.Count)];
    }
}