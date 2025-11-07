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

    //크로마대쉬 성공
    public static event Action<float> OnChromaDashSuccessed;
    public static void RaisedOnChromaDashSuccessed(float reward)
    {
        OnChromaDashSuccessed?.Invoke(reward);
    }
    //페널티 (땅과 색상 불일치)
    public static event Action<float> OnPenaltyWhenNoColorMatch;
    public static void RaisedOnPenaltyWhenNoColorMatch(float amount)
    {
        OnPenaltyWhenNoColorMatch?.Invoke(amount);
    }
    //퀵 슬롯 관련
    public static event Action<int, ItemData> OnQuickSlotChange;
    public static void RaisedOnQuickSlotChange(int slotIndex, ItemData itemData)
    {
        OnQuickSlotChange?.Invoke(slotIndex, itemData);
    }
}
