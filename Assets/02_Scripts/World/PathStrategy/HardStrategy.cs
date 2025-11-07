using UnityEngine;

public class HardStrategy : BasePathStrategy
{
    public override MapSegment NextSegment(GenerationContext context, MapThemeData themeData)
    {
        if (Random.value < 0.1f && themeData.rhythmEasySegments.Count > 0)
        {
            return GetSegmentFromList(themeData.rhythmEasySegments);
        }
        if (Random.value < 0.1f && themeData.rhythmHardSegments.Count > 0)
        {
            return GetSegmentFromList(themeData.rhythmHardSegments);
        }
        //테스트용
        if (Random.value < 0.1f && themeData.coinMediumSegments.Count > 0)
        {
            return GetSegmentFromList(themeData.coinMediumSegments);
        }
        if (Random.value < 0.3f && themeData.trapHardSegments.Count > 0)
        {
            return GetSegmentFromList(themeData.trapHardSegments);
        }
        if (Random.value < 0.3f && themeData.trapMediumSegments.Count > 0)
        {
            return GetSegmentFromList(themeData.trapMediumSegments);
        }
        if (Random.value < 0.5f && themeData.mediumSegments.Count > 0)
        {
            return GetSegmentFromList(themeData.mediumSegments);
        }

        return GetSegmentFromList(themeData.hardSegments);
    }
}
