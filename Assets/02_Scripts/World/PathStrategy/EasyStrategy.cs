using UnityEngine;

public class EasyStrategy : BasePathStrategy
{
    public override MapSegment NextSegment(GenerationContext context, MapThemeData themeData)
    {
        float roll = Random.value;
        int randomValue = Mathf.CeilToInt(roll * 10);

        switch (randomValue)
        {
            case 1:
            case 2:
                return GetSegmentFromList(themeData.coinEasySegments);
            case 3:
            case 4:
                return GetSegmentFromList(themeData.easySegments);
            case 5:
                return GetSegmentFromList(themeData.trapEasySegments);
            case 6:
                return GetSegmentFromList(themeData.rhythmEasySegments);
            case 7:
                return GetSegmentFromList(themeData.mediumSegments);
            case 8:
                return GetSegmentFromList(themeData.hardSegments);
            case 9:
                return GetSegmentFromList(themeData.trapMediumSegments);
            case 10:
                return GetSegmentFromList(themeData.coinMediumSegments);
            default:
                return GetSegmentFromList(themeData.easySegments);
        }
    }
}
