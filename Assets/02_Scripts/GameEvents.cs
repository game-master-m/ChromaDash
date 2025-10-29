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

    //ũ�θ��뽬 ����
    public static event Action<float> OnChromaDashSuccessed;
    public static void RaisedOnChromaDashSuccessed(float reward)
    {
        OnChromaDashSuccessed?.Invoke(reward);
    }
    //���Ƽ (���� ���� ����ġ)
    public static event Action<float> OnPenaltyWhenNoColorMatch;
    public static void RaisedOnPenaltyWhenNoColorMatch(float amount)
    {
        OnPenaltyWhenNoColorMatch?.Invoke(amount);
    }
    //�� ���� ����
    public static event Action<int, ItemData> OnQuickSlotChange;
    public static void RaisedOnQuickSlotChange(int slotIndex, ItemData itemData)
    {
        OnQuickSlotChange?.Invoke(slotIndex, itemData);
    }
}
