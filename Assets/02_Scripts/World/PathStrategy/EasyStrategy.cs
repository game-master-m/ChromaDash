using UnityEngine;

public class EasyStrategy : BasePathStrategy
{
    public override MapSegment NextSegment(GenerationContext context, MapThemeData themeData)
    {
        if (Random.value < 0.1f && themeData.rhythmEasySegments.Count > 0)
        {
            return GetSegmentFromList(themeData.rhythmEasySegments);
        }
        if (Random.value < 0.5f && themeData.coinEasySegments.Count > 0)
        {
            return GetSegmentFromList(themeData.coinEasySegments);
        }
        return GetSegmentFromList(themeData.easySegments);
    }
}
