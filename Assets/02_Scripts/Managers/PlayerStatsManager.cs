using System.Collections;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    [Header("수정할 데이터")]
    [SerializeField] private PlayerStatsData playerStatsData;
    [Header("난이도 설정")]
    [SerializeField] private int mediumModeChangeScore = 100;
    [SerializeField] private int hardModeChangeScore = 500;

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
    //score 관련
    [SerializeField] private FloatEventChannelSO onDistanceCheckPerCool;    //PlayerController 가 발행
    //sound 관련
    [SerializeField] private VoidEventChannelSO onBgmEnd;
    [SerializeField] private VoidEventChannelSO onReturnToLobby;        //GameManager 가 발행

    [Header("발행할 채널")]
    [SerializeField] private VoidEventChannelSO onPlayerDie;        //GameManager 가 구독
    [SerializeField] private EDifficultyModeEventChannelSO onDifficultModeChangeRequest; //LevelGenerator 가 구독

    //현재 난이도 저장 변수
    private EDifficultyMode eCurrentDifficultyMode = EDifficultyMode.Easy;

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
        //score 관련
        onDistanceCheckPerCool.OnEvent += HandleDistanceCheckForUpdateScore;
        playerStatsData.onScoreChange += DifficultyChange;
        //sound 관련
        //onBgmEnd.OnEvent += HandleBgmEnd;
        playerStatsData.onTimeGaugeChange += BgmChangeByTimeGuage;
        onReturnToLobby.OnEvent += HandleReturnToLobby;
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
        onDistanceCheckPerCool.OnEvent -= HandleDistanceCheckForUpdateScore;
        //onBgmEnd.OnEvent -= HandleBgmEnd;
        playerStatsData.onTimeGaugeChange -= BgmChangeByTimeGuage;
        playerStatsData.onScoreChange -= DifficultyChange;
        onReturnToLobby.OnEvent -= HandleReturnToLobby;
    }
    private void HandleReturnToLobby()
    {
        isPlaying = false;
        Managers.Sound.PlayBGM(EBgmName.Lobby, true);
        Time.timeScale = 1.0f;
    }
    private void DifficultyChange()
    {
        if (!isPlaying) return;
        if (playerStatsData.CurrentScore < mediumModeChangeScore)
        {
            //easy mode
            if (eCurrentDifficultyMode == EDifficultyMode.Easy) return;
            eCurrentDifficultyMode = EDifficultyMode.Easy;
            onDifficultModeChangeRequest.Rasied(eCurrentDifficultyMode);
        }
        else if (playerStatsData.CurrentScore >= mediumModeChangeScore && playerStatsData.CurrentScore < hardModeChangeScore)
        {
            //medium mode
            if (eCurrentDifficultyMode == EDifficultyMode.Medium) return;
            eCurrentDifficultyMode = EDifficultyMode.Medium;
            onDifficultModeChangeRequest.Rasied(eCurrentDifficultyMode);
            gaugeCostPerSec = 1.5f;
        }
        else if (playerStatsData.CurrentScore >= hardModeChangeScore)
        {
            //hard mode
            if (eCurrentDifficultyMode != EDifficultyMode.Hard && eCurrentDifficultyMode != EDifficultyMode.ChromaHard)
            {
                eCurrentDifficultyMode = EDifficultyMode.Hard;
                onDifficultModeChangeRequest.Rasied(eCurrentDifficultyMode);
                gaugeCostPerSec = 2.0f;
            }
            //chroma hard mode
            if (playerStatsData.BestScore > hardModeChangeScore &&
                playerStatsData.CurrentScore > playerStatsData.BestScore)
            {
                if (eCurrentDifficultyMode != EDifficultyMode.ChromaHard)
                {
                    eCurrentDifficultyMode = EDifficultyMode.ChromaHard;
                    onDifficultModeChangeRequest.Rasied(eCurrentDifficultyMode);
                    gaugeCostPerSec = 2.5f;
                }
            }
        }

    }
    private void BgmChangeByTimeGuage(float currentGuage, float maxGuage)
    {
        if (!isPlaying) return;
        if (currentGuage / maxGuage <= 0.2f && isPlaying && !playerStatsData.IsTimesUp)
        {
            Managers.Sound.PlayBGM(EBgmName.InGameLowGuage, false);
        }
        else
        {
            HandleBgmEnd();
        }
    }
    private void HandleBgmEnd()
    {
        if (isPlaying && !playerStatsData.IsTimesUp)
        {
            switch (eCurrentDifficultyMode)
            {
                case EDifficultyMode.Easy:
                    Managers.Sound.PlayBGM(EBgmName.InGameFull, false);
                    break;
                case EDifficultyMode.Medium:
                    Managers.Sound.PlayBGM(EBgmName.InGameMediumGuage, false);
                    break;
                case EDifficultyMode.Hard:
                    Managers.Sound.PlayBGM(EBgmName.InGameMediumGuage, false);
                    break;
                case EDifficultyMode.ChromaHard:
                    Managers.Sound.PlayBGM(EBgmName.InGameLowGuage, false);
                    break;
            }
        }
    }
    private void HandleDistanceCheckForUpdateScore(float distance)
    {
        playerStatsData.UpdateScore(Mathf.RoundToInt(distance / 10));
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

        Managers.Sound.PlayBGM(EBgmName.InGameFull, false);
    }
    public void HandleGameOver()
    {
        isPlaying = false;
        playerStatsData.GameOverScore();

        Managers.Sound.PlayBGM(EBgmName.InGameDie, true);
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

    IEnumerator UpdateGaugeCoolCo(float coolTime)
    {
        while (isPlaying)
        {
            if (isRealTimeGuageDecrease) yield return new WaitForSecondsRealtime(coolTime);
            else yield return new WaitForSeconds(coolTime);
            bool isTimesUp = playerStatsData.UpdataGauge(gaugeCostPerSec * coolTime);
            if (isTimesUp)
            {
                isPlaying = false;
                onPlayerDie.Raised();
            }
        }
    }

}
