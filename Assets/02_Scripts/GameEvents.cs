using System;

public static class GameEvents
{
    //Ÿ�Ӱ�����
    public static event Action<float, float> OnTimeGaugeChanged;
    public static void RaiseOnTimeGuageChanged(float currentGauge, float maxGauge)
    {
        OnTimeGaugeChanged?.Invoke(currentGauge, maxGauge);
    }

    //ĳ���� ����ȭ
    public static event Action<EChromaColor> OnChromaColorChanged;
    public static void RaiseOnChromaColorChanged(EChromaColor chromaColor)
    {
        OnChromaColorChanged?.Invoke(chromaColor);
    }
}
