using UnityEngine;

public class RhythmFocusStrategy : BasePathStrategy
{
    public override MapSegment NextSegment(GenerationContext context, MapThemeData themeData)
    {
        float roll = Random.value;

        if (roll < 0.2f && themeData.rhythmHardSegments.Count > 0)
        {
            return GetSegmentFromList(themeData.rhythmHardSegments);
        }
        if (0.2f <= roll && roll < 0.5f && themeData.rhythmMediumSegments.Count > 0)
        {
            return GetSegmentFromList(themeData.rhythmMediumSegments);
        }
        if (0.5f <= roll && roll < 0.7f && themeData.rhythmEasySegments.Count > 0)
        {
            return GetSegmentFromList(themeData.rhythmEasySegments);
        }
        return GetSegmentFromList(themeData.trapHardSegments);
    }
}
