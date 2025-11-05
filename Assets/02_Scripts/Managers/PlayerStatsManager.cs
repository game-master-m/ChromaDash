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
    //TimeSlowTrap관련
    [SerializeField] private FloatEventChannelSO onTimeSlowTrappedRequest;  //PlayerController 가 발행
    [SerializeField] private VoidEventChannelSO onTimeSlowExit;             //TimeSlowTrap 이 발행

    [Header("발행할 채널")]
    [SerializeField] private VoidEventChannelSO onPlayerDie;        //GameManager 가 구독

    private bool isPlaying = false;
    private Coroutine updateGuaugeCoolCo;

    private bool isRealTimeGuageDecrease = false;
    private void Start()
    {
        playerStatsData.Init();
    }

    private void OnEnable()
    {
        onChromaDashSuccess.OnEvent += HandleSuccessReward;
        onHealPotionRequest.OnEvent += HandleSuccessReward;
        onPenaltyWhenNoColorMatch.OnEvent += HandlePenalty;
        onChromaColorChangeRequest.OnEvent += HandleColorChange;

        //GameManager 관련
        onGameStart.OnEvent += HandleGameStart;
        onGameOver.OnEvent += HandleGameOver;
        //TimeSlowTrap 관련
        onTimeSlowExit.OnEvent += HandleTimeSlowTrapEscape;
        onTimeSlowTrappedRequest.OnEvent += HandleTimeSlowTrapped;
        onChromaDashSuccess.OnEvent += HandleTimeSlowTrapEscape;
    }
    private void OnDisable()
    {
        onChromaDashSuccess.OnEvent -= HandleSuccessReward;
        onHealPotionRequest.OnEvent -= HandleSuccessReward;
        onPenaltyWhenNoColorMatch.OnEvent -= HandlePenalty;
        onChromaColorChangeRequest.OnEvent -= HandleColorChange;
        onGameStart.OnEvent -= HandleGameStart;
        onGameOver.OnEvent -= HandleGameOver;
        onTimeSlowTrappedRequest.OnEvent -= HandleTimeSlowTrapped;
        onTimeSlowExit.OnEvent -= HandleTimeSlowTrapEscape;
        onChromaDashSuccess.OnEvent -= HandleTimeSlowTrapEscape;
    }
    private void HandleTimeSlowTrapped(float slowFactor)
    {
        //TimeScalse은 느려지는데 TimeGuage는 그대로 감소
        isRealTimeGuageDecrease = true;
        Time.timeScale = slowFactor;
    }
    private void HandleTimeSlowTrapEscape(float garbage)
    {
        isRealTimeGuageDecrease = false;
        Time.timeScale = 1.0f;
    }
    private void HandleTimeSlowTrapEscape()
    {
        isRealTimeGuageDecrease = false;
        Time.timeScale = 1.0f;
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
            if (isRealTimeGuageDecrease) yield return new WaitForSecondsRealtime(deltTime);
            else yield return new WaitForSeconds(deltTime);
            bool isTimesUp = playerStatsData.UpdataGauge(gaugeCostPerSec * deltTime);
            if (isTimesUp)
            {
                isPlaying = false;
                onPlayerDie.Raised();
            }
        }
    }
}
