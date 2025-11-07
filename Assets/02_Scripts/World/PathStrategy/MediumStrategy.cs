using UnityEngine;

public class MediumStrategy : BasePathStrategy
{
    public override MapSegment NextSegment(GenerationContext context, MapThemeData themeData)
    {
        float roll = Random.value;
        int randomValue = Mathf.CeilToInt(roll * 10);
        switch (randomValue)
        {
            case 1:
                return GetSegmentFromList(themeData.coinEasySegments);
            case 2:
                return GetSegmentFromList(themeData.coinMediumSegments);
            case 3:
                return GetSegmentFromList(themeData.coinHardSegments);
            case 4:
                return GetSegmentFromList(themeData.mediumSegments);
            case 5:
                return GetSegmentFromList(themeData.trapHardSegments);
            case 6:
                return GetSegmentFromList(themeData.trapMediumSegments);
            case 7:
                return GetSegmentFromList(themeData.trapEasySegments);
            case 8:
                return GetSegmentFromList(themeData.rhythmEasySegments);
            case 9:
                return GetSegmentFromList(themeData.rhythmMediumSegments);
            case 10:
                return GetSegmentFromList(themeData.rhythmHardSegments);
            default:
                return GetSegmentFromList(themeData.mediumSegments);
        }

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
