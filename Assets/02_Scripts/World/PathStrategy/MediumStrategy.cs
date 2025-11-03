using UnityEngine;

public class MediumStrategy : BasePathStrategy
{
    public override MapSegment NextSegment(GenerationContext context, MapThemeData themeData)
    {
        if (Random.value < 0.1f && themeData.rhythmMediumSegments.Count > 0)
        {
            return GetSegmentFromList(themeData.rhythmMediumSegments);
        }
        if (Random.value < 0.5f && themeData.coinMediumSegments.Count > 0)
        {
            return GetSegmentFromList(themeData.coinMediumSegments);
        }
        return GetSegmentFromList(themeData.mediumSegments);
    }
}
