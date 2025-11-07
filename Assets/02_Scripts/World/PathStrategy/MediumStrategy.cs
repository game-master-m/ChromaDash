using UnityEngine;

public class MediumStrategy : BasePathStrategy
{
    public override MapSegment NextSegment(GenerationContext context, MapThemeData themeData)
    {
        if (Random.value < 0.1f && themeData.rhythmMediumSegments.Count > 0)
        {
            return GetSegmentFromList(themeData.rhythmMediumSegments);
        }
        if (Random.value < 0.15f && themeData.coinMediumSegments.Count > 0)
        {
            return GetSegmentFromList(themeData.coinMediumSegments);
        }
        if (Random.value < 0.2f && themeData.trapMediumSegments.Count > 0)
        {
            return GetSegmentFromList(themeData.trapMediumSegments);
        }
        return GetSegmentFromList(themeData.mediumSegments);
    }
}
