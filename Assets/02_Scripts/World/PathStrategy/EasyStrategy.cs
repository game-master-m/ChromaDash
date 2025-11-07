using UnityEngine;

public class EasyStrategy : BasePathStrategy
{
    public override MapSegment NextSegment(GenerationContext context, MapThemeData themeData)
    {
        if (Random.value < 0.1f && themeData.rhythmEasySegments.Count > 0)
        {
            return GetSegmentFromList(themeData.rhythmEasySegments);
        }
        if (Random.value < 0.15f && themeData.coinEasySegments.Count > 0)
        {
            return GetSegmentFromList(themeData.coinEasySegments);
        }
        if (Random.value < 0.2f && themeData.trapEasySegments.Count > 0)
        {
            return GetSegmentFromList(themeData.trapEasySegments);
        }
        return GetSegmentFromList(themeData.easySegments);
    }
}
