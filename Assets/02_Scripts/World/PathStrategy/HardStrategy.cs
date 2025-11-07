using UnityEngine;

public class HardStrategy : BasePathStrategy
{
    public override MapSegment NextSegment(GenerationContext context, MapThemeData themeData)
    {
        float roll = Random.value;
        int randomValue = Mathf.CeilToInt(roll * 10);
        switch (randomValue)
        {
            case 1:
                return GetSegmentFromList(themeData.rhythmMediumSegments);
            case 2:
                return GetSegmentFromList(themeData.rhythmHardSegments);
            case 3:
                return GetSegmentFromList(themeData.coinMediumSegments);
            case 4:
                return GetSegmentFromList(themeData.coinHardSegments);
            case 5:
            case 6:
                return GetSegmentFromList(themeData.trapHardSegments);
            case 7:
                return GetSegmentFromList(themeData.trapMediumSegments);
            case 8:
                return GetSegmentFromList(themeData.trapEasySegments);
            case 9:
                return GetSegmentFromList(themeData.mediumSegments);
            case 10:
                return GetSegmentFromList(themeData.hardSegments);
            default:
                break;
        }

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
