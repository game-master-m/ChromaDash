using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    [Header("수정할 데이터")]
    [SerializeField] private PlayerStatsData playerStatsData;

    [Header("게이지 설정")]
    [SerializeField] private float gaugeCostPerSec = 1.0f;

    [Header("구독할 채널")]
    [SerializeField] private FloatEventChannelSO onChromaDashSuccess;
    [SerializeField] private FloatEventChannelSO onPenaltyWhenNoColorMatch;
    [SerializeField] private EChromaColorEventChannelSO onChromaColorChangeRequest;
    [SerializeField] private FloatEventChannelSO onHealPotionRequest;

    [Header("발행할 채널")]
    [SerializeField] private VoidEventChannelSO onPlayerDie;

    private void Start()
    {
        playerStatsData.Init();
    }

    private void OnEnable()
    {
        onChromaDashSuccess.onEvent += HandleSuccessReward;
        onHealPotionRequest.onEvent += HandleSuccessReward;
        onPenaltyWhenNoColorMatch.onEvent += HandlePenalty;
        onChromaColorChangeRequest.onEvent += HandleColorChange;
    }
    private void OnDisable()
    {
        onChromaDashSuccess.onEvent -= HandleSuccessReward;
        onHealPotionRequest.onEvent -= HandleSuccessReward;
        onPenaltyWhenNoColorMatch.onEvent -= HandlePenalty;
        onChromaColorChangeRequest.onEvent -= HandleColorChange;
    }
    private void Update()
    {
        bool isTimesUp = playerStatsData.UpdataGauge(Time.deltaTime, gaugeCostPerSec);
        if (isTimesUp)
        {
            onPlayerDie.Raised();
        }
    }

    private void HandleSuccessReward(float amount)
    {
        playerStatsData.RecoverTime(amount);
    }
    private void HandlePenalty(float amount)
    {
        playerStatsData.TakePenalty(amount);
    }
    private void HandleColorChange(EChromaColor color)
    {
        playerStatsData.ChangeColor(color);
    }
}
