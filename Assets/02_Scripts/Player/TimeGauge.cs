using System.Collections;
using UnityEngine;

public class TimeGauge : MonoBehaviour
{
    [Header("타임게이지 설정")]
    [SerializeField] private float maxGauge = 100f;
    [SerializeField] private float gaugeCostPerSec = 1.0f;
    [SerializeField] private float gaugeUpdateCool = 0.5f;

    public float CurrentGauge { get; private set; }

    public bool IsTimesUp { get; private set; } = false;
    void Start()
    {
        CurrentGauge = maxGauge;
        Coroutine changeGuageEveryT = StartCoroutine(ChangeUIGaugeEveryTCo(gaugeUpdateCool));
        GameEvents.OnChromaDashSuccessed += RecoverTimeWhenChromaSuccess;
        GameEvents.OnPenaltyWhenNoColorMatch += TakePenaltyWhenColorNoMatch;
    }
    private void OnDestroy()
    {
        GameEvents.OnChromaDashSuccessed -= RecoverTimeWhenChromaSuccess;
        GameEvents.OnPenaltyWhenNoColorMatch -= TakePenaltyWhenColorNoMatch;
    }
    void Update()
    {
        if (CurrentGauge > 0)
        {
            CurrentGauge -= gaugeCostPerSec * Time.deltaTime;
            if (CurrentGauge < 0) CurrentGauge = 0;
            if (CurrentGauge == 0)
            {
                //죽음
                IsTimesUp = true;
                return;
            }
        }
    }
    public void TakePenaltyWhenColorNoMatch(float amount)
    {
        if (CurrentGauge > 0)
        {
            CurrentGauge -= amount;
            if (CurrentGauge < 0) CurrentGauge = 0;
            GameEvents.RaiseOnTimeGuageChanged(CurrentGauge, maxGauge);
        }
    }
    public void RecoverTimeWhenChromaSuccess(float amount)
    {
        CurrentGauge += amount;
        if (CurrentGauge > maxGauge) CurrentGauge = maxGauge;
        GameEvents.RaiseOnTimeGuageChanged(CurrentGauge, maxGauge);
    }
    IEnumerator ChangeUIGaugeEveryTCo(float t)
    {
        while (true)
        {
            GameEvents.RaiseOnTimeGuageChanged(CurrentGauge, maxGauge);
            yield return new WaitForSeconds(t);
        }
    }
}
