using System;

public static class GameEvents
{
    //타임게이지
    public static event Action<float, float> OnTimeGaugeChanged;
    public static void RaiseOnTimeGuageChanged(float currentGauge, float maxGauge)
    {
        OnTimeGaugeChanged?.Invoke(currentGauge, maxGauge);
    }

    //캐릭터 색상변화
    public static event Action<EChromaColor> OnChromaColorChanged;
    public static void RaiseOnChromaColorChanged(EChromaColor chromaColor)
    {
        OnChromaColorChanged?.Invoke(chromaColor);
    }
}
