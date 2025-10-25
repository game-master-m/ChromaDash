using UnityEngine;

public class RhythmFocusStrategy : BasePathStrategy
{
    public override MapSegment NextSegment(GenerationContext context, MapThemeData themeData)
    {
        float roll = Random.value;

        if (roll < 0.2f && themeData.hardSegments.Count > 0)
        {
            return GetSegmentFromList(context, themeData.hardSegments);
        }
        if (0.2f <= roll && roll < 0.5f && themeData.mediumSegments.Count > 0)
        {
            return GetSegmentFromList(context, themeData.mediumSegments);
        }
        if (0.5f <= roll && roll < 0.7f && themeData.easySegments.Count > 0)
        {
            return GetSegmentFromList(context, themeData.easySegments);
        }
        return GetSegmentFromList(context, themeData.rhythmSegments);
    }
}
