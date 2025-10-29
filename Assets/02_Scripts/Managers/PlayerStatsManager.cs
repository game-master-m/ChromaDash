using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    [Header("������ ������")]
    [SerializeField] private PlayerStatsData playerStatsData;

    [Header("������ ����")]
    [SerializeField] private float gaugeCostPerSec = 1.0f;

    [Header("������ ä��")]
    [SerializeField] private FloatEventChannelSO onChromaDashSuccess;
    [SerializeField] private FloatEventChannelSO onPenaltyWhenNoColorMatch;
    [SerializeField] private EChromaColorEventChannelSO onChromaColorChangeRequest;
    [SerializeField] private FloatEventChannelSO onHealPotionRequest;

    [Header("������ ä��")]
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
