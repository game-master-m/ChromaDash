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
    Default, TransparentFX, IgnoreRaycast, Ground, Water, UI
}
public enum EPatterDifficulty
{
    Easy, Medium, Hard
}
public enum EJumpDifficulty
{
    Easy,        // �⺻ ����
    Medium,      // 2�� ����
    Hard,        // ~ ����  
    ChromaHard  // ũ�θ� �ν�Ʈ + 2�� ����
}