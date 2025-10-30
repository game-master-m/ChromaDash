using System.Collections;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    [Header("수정할 데이터")]
    [SerializeField] private PlayerStatsData playerStatsData;

    [Header("게이지 설정")]
    [SerializeField] private float gaugeCostPerSec = 1.0f;
    [SerializeField] private float updateGuageUIDeltaTime = 0.5f;

    [Header("구독할 채널")]
    [SerializeField] private FloatEventChannelSO onChromaDashSuccess;
    [SerializeField] private FloatEventChannelSO onPenaltyWhenNoColorMatch;
    [SerializeField] private EChromaColorEventChannelSO onChromaColorChangeRequest;
    //아이템사용
    [SerializeField] private FloatEventChannelSO onHealPotionRequest;
    //GameManager관련
    [SerializeField] private VoidEventChannelSO onGameStart;
    [SerializeField] private VoidEventChannelSO onGameOver;

    [Header("발행할 채널")]
    [SerializeField] private VoidEventChannelSO onPlayerDie;

    private bool isPlaying = false;
    private Coroutine updateGuaugeCoolCo;
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

        //GameManager 관련
        onGameStart.OnEvent += HandleGameStart;
        onGameOver.OnEvent += HandleGameOver;
    }
    private void OnDisable()
    {
        onChromaDashSuccess.onEvent -= HandleSuccessReward;
        onHealPotionRequest.onEvent -= HandleSuccessReward;
        onPenaltyWhenNoColorMatch.onEvent -= HandlePenalty;
        onChromaColorChangeRequest.onEvent -= HandleColorChange;
        onGameStart.OnEvent -= HandleGameStart;
        onGameOver.OnEvent -= HandleGameOver;
    }
    public void HandleGameStart()
    {
        playerStatsData.Init();
        isPlaying = true;
        if (updateGuaugeCoolCo != null) StopCoroutine(updateGuaugeCoolCo);
        updateGuaugeCoolCo = StartCoroutine(UpdateGaugeCoolCo(updateGuageUIDeltaTime));
    }
    public void HandleGameOver()
    {
        isPlaying = false;
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

    IEnumerator UpdateGaugeCoolCo(float deltTime)
    {
        while (isPlaying)
        {
            yield return new WaitForSeconds(deltTime);
            bool isTimesUp = playerStatsData.UpdataGauge(gaugeCostPerSec * deltTime);
            if (isTimesUp)
            {
                isPlaying = false;
                onPlayerDie.Raised();
            }
        }
    }
}
