using UnityEngine;

public class EasyStrategy : BasePathStrategy
{
    public override MapSegment NextSegment(GenerationContext context, MapThemeData themeData)
    {
        if (Random.value < 0.1f && themeData.rhythmSegments.Count > 0)
        {
            return GetSegmentFromList(context, themeData.rhythmSegments);
        }
        return GetSegmentFromList(context, themeData.easySegments);
    }
}
