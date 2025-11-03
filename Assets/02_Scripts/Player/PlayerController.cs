using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //component
    public PlayerMove Move { get; private set; }
    public PlayerAnim Anim { get; private set; }

    //state
    public StateMachine StateMachine { get; private set; }
    private GroundState groundState;
    private JumpOnGroundState jumpOnGroundState;
    private AirState airState;
    private JumpInAirState jumpInAirState;
    private ReadyChromaDashState readyChromaDashState;
    private ChromaDashState chromaDashState;
    private HurtState hurtState;
    //input
    public InputManager Input { get; private set; }

    //Inspector 참조
    [SerializeField] private Transform[] groundChecks;

    //Inspector 조절 변수
    [Header("그라운드 체크용")]
    [SerializeField] private float intoGroundDelay = 0.2f;
    [SerializeField] private float rayLengthForGroundCheck = 0.1f;
    [SerializeField] private float rayLengthForChromaDash = 0.25f;
    [Header("물리값")]
    [SerializeField] private float runSpeed = 5.0f;
    [SerializeField] private float hurtSlowSpeed = 2.0f;
    [SerializeField] private float chromaDashForce = 5.0f;
    [SerializeField] private float jumpForceFirst = 5.0f;
    [SerializeField] private float jumpForceContinue = 3.0f;
    [SerializeField] private float jumpForceHover = 1.0f;
    [SerializeField] private float jumpFirceContinueDuration = 1.2f;
    [Header("크로마대쉬 관련")]
    [SerializeField] private float chromaDashTime = 1.0f;
    [SerializeField] private float chromaToRunTransitionTValue = 2.0f;
    [SerializeField] private float hurtToRunTransitionTValue = 2.0f;
    [Header("게이지 관련")]
    [SerializeField] private float chromaDashSuccessRewardAmount = 5.0f;
    [SerializeField] private float penaltyColorNoMatchAmount = 10.0f;
    [Header("이벤트 발행")]
    [SerializeField] private FloatEventChannelSO onPenaltyWhenNoColorMatch;
    [SerializeField] private FloatEventChannelSO onChromaDashSuccess;
    [SerializeField] private FloatEventChannelSO onTimeSlowTrappedRequest;          //PlayerStatsManager에서 구독
    [Header("이벤트 구독")]
    [SerializeField] private VoidEventChannelSO onColorForcedChangeLeft;        //ColorChangeTrap에서 발행
    [SerializeField] private VoidEventChannelSO onColorForcedChangeRight;       //ColorChangeTrap에서 발행
    [SerializeField] private FloatEventChannelSO onTimeSlowEnter;               //TimeSlowTrap에서 발행
    [SerializeField] private VoidEventChannelSO onTimeSlowExit;                 //TimeSlowTrap에서 발행

    //인스펙터 조절변수 Getter
    public FloatEventChannelSO OnPenaltyWhenNoColorMatch { get { return onPenaltyWhenNoColorMatch; } }
    public FloatEventChannelSO OnChromaDashSuccess { get { return onChromaDashSuccess; } }
    public float ChromaDashSuccesRewardAmount { get { return chromaDashSuccessRewardAmount; } }
    public float PenaltyColorNoMatchAmount { get { return penaltyColorNoMatchAmount; } }
    public float HurtSlowSpeed { get { return hurtSlowSpeed; } }
    public float ChromaDashForce { get { return chromaDashForce; } }
    public float JumpForceFirst { get { return jumpForceFirst; } }
    public float JumpForceContinue { get { return jumpForceContinue; } }
    public float JumpForceHover { get { return jumpForceHover; } }
    public float JumpFirceContinueDuration { get { return jumpFirceContinueDuration; } }
    public float ChromaDashTime { get { return chromaDashTime; } }

    //멤버
    public bool IsChromaDash { get; set; } = false;
    public bool IsGround { get; private set; } = false;
    public bool IsDelayedGround { get; private set; } = false;
    public bool IsInChromaDashDistance { get; private set; } = false;
    public bool WasJumpedOnGround { get; set; } = false;
    public bool WasJumpedInAir { get; private set; } = false;
    public bool WasSecondJumpedInAir { get; private set; } = false;
    public EChromaColor EDetectedColorFromSeg { get; private set; } = EChromaColor.None;
    public EChromaColor ECurrentColor { get; private set; } = EChromaColor.Red;

    private bool isChromaDashBetweenGroundToDelayed = false;

    //맵 효과
    private bool isInTimeSlowTrap = false;
    private float timeSlowFactor;

    private void Awake()
    {
        //컴포넌트
        Move = GetComponent<PlayerMove>();
        Anim = GetComponent<PlayerAnim>();
        //인풋매니저 등록해서 사용

        //초기 색상
        ECurrentColor = EChromaColor.Red;

        //States
        StateMachine = new StateMachine();
        groundState = new GroundState(this);
        jumpOnGroundState = new JumpOnGroundState(this);
        airState = new AirState(this);
        jumpInAirState = new JumpInAirState(this, airState);
        readyChromaDashState = new ReadyChromaDashState(this, airState);
        chromaDashState = new ChromaDashState(this);
        hurtState = new HurtState(this);

        AddTransitions();

        //
    }
    private void Start()
    {
        Input = Managers.Input;
        StateMachine.ChangeState(airState);

        //이벤트 구독
        onColorForcedChangeLeft.OnEvent += ChangeColorAsKeyLeft;
        onColorForcedChangeRight.OnEvent += ChangeColorAsKeyRight;
        onTimeSlowEnter.OnEvent += OnTimeSlowEnter;
        onTimeSlowExit.OnEvent += OnTimeSlowExit;
    }
    private void OnDestroy()
    {
        onColorForcedChangeLeft.OnEvent -= ChangeColorAsKeyLeft;
        onColorForcedChangeRight.OnEvent -= ChangeColorAsKeyRight;
        onTimeSlowEnter.OnEvent -= OnTimeSlowEnter;
        onTimeSlowExit.OnEvent -= OnTimeSlowExit;
    }
    void Update()
    {
        GroundCheck();
        StateMachine.Update();
        if (Input.IsColorChangeLeftPressed) ChangeColorAsKeyLeft();
        if (Input.IsColorChangeRightPressed) ChangeColorAsKeyRight();


        //test용 reLoad
        if (Input.IsReLoadPressed) SceneManager.LoadScene("PlayScene");
    }
    private void FixedUpdate()
    {
        StateMachine.FixedUpdate();
        RunSpeedControl();
    }
    #region Transition Method
    private void AddTransitions()
    {
        StateMachine.AddTransition(airState, groundState,
            () => (IsGround && !(StateMachine.CurrentState is ReadyChromaDashState) && (ECurrentColor == EDetectedColorFromSeg)) || IsDelayedGround);
        StateMachine.AddTransition(groundState, airState, () => !IsGround);
        StateMachine.AddTransition(groundState, jumpOnGroundState, () => Input.IsJumpPressed);
        StateMachine.AddTransition(jumpOnGroundState, airState, () => jumpOnGroundState.DoChangeStateJumpOnGroundToAirIdle);
        StateMachine.AddTransition(jumpOnGroundState, jumpInAirState, new Func<bool>(DoChangeStateJumpOnGroundToJump));
        StateMachine.AddTransition(jumpInAirState, airState, () => jumpInAirState.DoChangeStateJumpInAirToAirIdle);
        StateMachine.AddTransition(airState, jumpInAirState, new Func<bool>(DoChangeStateAirToJump));
        StateMachine.AddTransition(airState, readyChromaDashState, () => !(StateMachine.CurrentState is ReadyChromaDashState) && !(StateMachine.CurrentState is JumpInAirState)
                && IsInChromaDashDistance && ECurrentColor != EDetectedColorFromSeg && EDetectedColorFromSeg != EChromaColor.None);

        StateMachine.AddTransition(readyChromaDashState, chromaDashState,
            () => readyChromaDashState.CanChromaDashReady && (IsDelayedGround || IsGround || isChromaDashBetweenGroundToDelayed));
        StateMachine.AddTransition(readyChromaDashState, jumpInAirState, new Func<bool>(DoChangeStateAirToJump));
        StateMachine.AddTransition(chromaDashState, groundState, () => !IsChromaDash);
        StateMachine.AddTransition(chromaDashState, jumpOnGroundState, () => Input.IsJumpPressed && IsGround);
        //StateMachine.AddTransition(chromaDashState, jumpInAirState, () => Input.IsJumpPressed && !IsGround);
        StateMachine.AddTransition(chromaDashState, airState, () => !IsGround);
        StateMachine.AddTransition(hurtState, airState, () => hurtState.IsHurtEnd);

        //Any
        StateMachine.AddAnyTransition(hurtState,
            () => IsDelayedGround && ECurrentColor != EDetectedColorFromSeg && !(StateMachine.CurrentState is HurtState));
    }
    public bool DoChangeStateJumpOnGroundToJump()
    {
        if (Input.IsJumpPressed)
        {
            WasJumpedInAir = true;
            return true;
        }
        else return false;
    }
    public bool DoChangeStateAirToJump()
    {
        if (WasJumpedOnGround)
        {
            if (Input.IsJumpPressed && !WasJumpedInAir)
            {
                WasJumpedInAir = true;
                return true;
            }
            else return false;
        }
        else
        {
            if (Input.IsJumpPressed && !WasJumpedInAir && !WasSecondJumpedInAir)
            {
                WasJumpedInAir = true;
                return true;
            }
            else if (Input.IsJumpPressed && WasJumpedInAir && !WasSecondJumpedInAir)
            {
                WasSecondJumpedInAir = true;
                return true;
            }
            else return false;
        }
    }

    #endregion
    private void RunSpeedControl()
    {
        if (hurtState.IsHurtSlow)
        {
            Move.VelocityXLerpEaseIn(Move.GetVelocityX(), runSpeed, hurtToRunTransitionTValue * Time.fixedDeltaTime);
            if (Move.GetVelocityX() > runSpeed - 0.1f) hurtState.IsHurtSlow = false;
        }
        else
        {
            if (!IsChromaDash)
            {
                if (Move.GetVelocityX() > runSpeed + 0.1f)
                {
                    Move.VelocityXLerpEaseIn(Move.GetVelocityX(), runSpeed, chromaToRunTransitionTValue * Time.fixedDeltaTime);
                }
                else { Move.SetVelocityX(runSpeed); }
            }
        }
    }

    #region 이벤트 핸들러들
    private void OnTimeSlowEnter(float tFromTimeSlowTrap)
    {
        isInTimeSlowTrap = true;
        timeSlowFactor = tFromTimeSlowTrap;
    }
    private void OnTimeSlowExit()
    {
        isInTimeSlowTrap = false;
    }

    #endregion

    #region 감지들
    IEnumerator IntoGroundDelay(float t)
    {
        yield return new WaitForSeconds(t);
        IsDelayedGround = true;
    }
    private void GroundCheck()
    {
        bool result = false;
        for (int i = 0; i < groundChecks.Length; i++)
        {
            RaycastHit2D col = Physics2D.Raycast(groundChecks[i].position, Vector2.down, rayLengthForGroundCheck,
                LayerManager.GetLayerMask(ELayerName.Ground));
            if (col.collider != null)
            {
                result = true;
                break;
            }
        }
        if (!result)
        {
            IsGround = false;
            IsDelayedGround = false;
            return;
        }
        if (!IsDelayedGround && !IsGround && result)
        {
            StartCoroutine(IntoGroundDelay(intoGroundDelay));
        }
        IsGround = result;
        if (IsGround && !readyChromaDashState.CanChromaDashReady && (ECurrentColor == EDetectedColorFromSeg))
        {
            //점프 조건 초기화!
            WasJumpedInAir = false;
            WasSecondJumpedInAir = false;
        }
        if (IsDelayedGround)
        {
            //점프 조건 초기화!
            WasJumpedInAir = false;
            WasSecondJumpedInAir = false;
            //크로마 감지들 초기화
            IsInChromaDashDistance = false;
            //시간감속 트랩 발동 ---------------
            if (isInTimeSlowTrap && !IsChromaDash) onTimeSlowTrappedRequest.Raised(timeSlowFactor);
            else if (isInTimeSlowTrap && IsChromaDash) isInTimeSlowTrap = false;
            // -------------------------------
        }
    }
    //AirState FixedUpdate()에서 검사
    public void ChromaDashCheck()
    {
        bool result = false;
        for (int i = 0; i < groundChecks.Length; i++)
        {
            RaycastHit2D col = Physics2D.Raycast(groundChecks[i].position, Vector2.down, rayLengthForChromaDash,
                LayerManager.GetLayerMask(ELayerName.Ground));
            if (col.collider != null)
            {
                if (col.collider.TryGetComponent<SegmentInfo>(out SegmentInfo info))
                {
                    EDetectedColorFromSeg = info.eColorForChromaDash;
                }
                result = true;
                break;
            }
        }
        if (!result) EDetectedColorFromSeg = EChromaColor.None;
        IsInChromaDashDistance = result;
    }
    #endregion

    #region 플레이어 색상변경 - 어디서든 적용을 위해 PlayerController에서 변경
    private void ChangeColorAsKeyLeft()
    {
        switch (ECurrentColor)
        {
            case EChromaColor.Red:
                ECurrentColor = EChromaColor.Green;
                break;
            case EChromaColor.Blue:
                ECurrentColor = EChromaColor.Red;
                break;
            case EChromaColor.Green:
                ECurrentColor = EChromaColor.Blue;
                break;
            default:
                ECurrentColor = EChromaColor.Red;
                break;
        }
        Anim.ChangeColor(ECurrentColor);
        GameEvents.RaiseOnChromaColorChanged(ECurrentColor);
        if (readyChromaDashState.CanChromaDashReady && IsGround && !IsDelayedGround && ECurrentColor == EDetectedColorFromSeg)
        {
            isChromaDashBetweenGroundToDelayed = true;
        }
    }
    private void ChangeColorAsKeyRight()
    {
        switch (ECurrentColor)
        {
            case EChromaColor.Red:
                ECurrentColor = EChromaColor.Blue;
                break;
            case EChromaColor.Blue:
                ECurrentColor = EChromaColor.Green;
                break;
            case EChromaColor.Green:
                ECurrentColor = EChromaColor.Red;
                break;
            default:
                ECurrentColor = EChromaColor.Red;
                break;
        }
        Anim.ChangeColor(ECurrentColor);
        GameEvents.RaiseOnChromaColorChanged(ECurrentColor);
    }
    #endregion

    #region 기즈모들
    private void OnDrawGizmos()
    {

        foreach (Transform groundCheck in groundChecks)
        {
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + new Vector3(0, -rayLengthForChromaDash, 0));
            RaycastHit2D col = Physics2D.Raycast(groundCheck.position, Vector2.down, rayLengthForChromaDash,
                LayerManager.GetLayerMask(ELayerName.Ground));
            if (col.collider != null) Gizmos.color = Color.red;
            else Gizmos.color = Color.gray;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + new Vector3(0, -rayLengthForChromaDash, 0));
        }

        foreach (Transform groundCheck in groundChecks)
        {
            RaycastHit2D col = Physics2D.Raycast(groundCheck.position, Vector2.down, rayLengthForGroundCheck,
                LayerManager.GetLayerMask(ELayerName.Ground));
            if (col.collider != null) Gizmos.color = Color.blue;
            else Gizmos.color = Color.yellow;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + new Vector3(0, -rayLengthForGroundCheck, 0));
        }
    }
    #endregion
}
