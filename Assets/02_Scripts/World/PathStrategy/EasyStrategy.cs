using UnityEngine;

public class EasyStrategy : BasePathStrategy
{
    public override MapSegment NextSegment(GenerationContext context, MapThemeData themeData)
    {
        if (Random.value < 0.1f && themeData.rhythmSegments.Count > 0)
        {
            return GetSegmentFromList(themeData.rhythmSegments);
        }
        return GetSegmentFromList(themeData.easySegments);
    }
}
