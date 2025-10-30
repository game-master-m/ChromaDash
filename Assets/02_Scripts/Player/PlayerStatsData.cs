using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PlayerStatsData", menuName = "ChromaDash/GameData/PlayerStatsData")]
public class PlayerStatsData : ScriptableObject
{
    [Header("타임게이지 설정")]
    [SerializeField] private float maxGauge = 100.0f;
    public float MaxGauge { get { return maxGauge; } }
    public float CurrentGauge { get; private set; }

    public EChromaColor CurrentChromaColor { get; private set; } = EChromaColor.Red;
    public bool IsTimesUp { get; private set; } = false;

    //게이지 변경 이벤트 UI가 구독
    public event Action<float, float> onTimeGaugeChange;
    public event Action<EChromaColor> onChromaColorChange;

    public void Init()
    {
        CurrentGauge = maxGauge;
        IsTimesUp = false;
        ChangeColor(EChromaColor.Red);

        onTimeGaugeChange?.Invoke(CurrentGauge, maxGauge);
    }
    public bool UpdataGauge(float costPerSec)
    {
        if (IsTimesUp) return true;
        CurrentGauge -= costPerSec;
        if (CurrentGauge < 0) CurrentGauge = 0;

        onTimeGaugeChange?.Invoke(CurrentGauge, maxGauge);

        if (CurrentGauge == 0)
        {
            IsTimesUp = true;
            return true;
        }
        else return false;
    }
    public void ChangeColor(EChromaColor color)
    {
        if (CurrentChromaColor == color) return;
        CurrentChromaColor = color;
        onChromaColorChange?.Invoke(CurrentChromaColor);
    }
    public void TakePenalty(float amount)
    {
        if (CurrentGauge > 0)
        {
            CurrentGauge -= amount;
            if (CurrentGauge < 0) CurrentGauge = 0;
            onTimeGaugeChange?.Invoke(CurrentGauge, maxGauge);
        }
    }
    public void RecoverTime(float amount)
    {
        if (IsTimesUp) return;
        CurrentGauge += amount;
        if (CurrentGauge > maxGauge) CurrentGauge = maxGauge;
        onTimeGaugeChange?.Invoke(CurrentGauge, maxGauge);
    }
}
