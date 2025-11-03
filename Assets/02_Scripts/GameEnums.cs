#region static LayerManager
public static class LayerManager
{
    public static int GetLayerMask(ELayerName layerName)
    {
        return 1 << (int)layerName;
    }
    public static int GetLayerMask(params ELayerName[] layerNames)
    {
        int result = 0;
        foreach (var layerName in layerNames)
        {
            result |= GetLayerMask(layerName);
        }
        return result;
    }
}
#endregion
public enum EChromaColor
{
    None, Red, Blue, Green
}
public enum ELayerName
{
    Default, TransparentFX, IgnoreRaycast, Ground, Water, UI, Player, Trap
}
public enum EPatterDifficulty
{
    Easy, Medium, Hard
}
public enum EJumpDifficulty
{
    Easy,        // 기본 점프
    Medium,      // 2단 점프
    Hard,        // ~ 사이  
    ChromaHard  // 크로마 부스트 + 2단 점프
}

public enum EItemType
{
    None,
    Heal,
    Shield,
    Rewind,
    SlowHeal,
    SpeedUp,
    SmallHeal
}

public enum EColorChangeTrap
{
    Right, Left
}