using UnityEngine;

public class RhythmFocusStrategy : BasePathStrategy
{
    public override MapSegment NextSegment(GenerationContext context, MapThemeData themeData)
    {
        float roll = Random.value;
        int randomValue = Mathf.CeilToInt(roll * 10);
        switch (randomValue)
        {
            case 1:
            case 2:
                return GetSegmentFromList(themeData.rhythmHardSegments);
            case 3:
            case 4:
                return GetSegmentFromList(themeData.rhythmMediumSegments);
            case 5:
                return GetSegmentFromList(themeData.trapMediumSegments);
            case 6:
            case 7:
                return GetSegmentFromList(themeData.trapHardSegments);
            case 8:
                return GetSegmentFromList(themeData.hardSegments);
            case 9:
                return GetSegmentFromList(themeData.mediumSegments);
            case 10:
                return GetSegmentFromList(themeData.easySegments);
            default:
                break;
        }

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
