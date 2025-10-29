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

    //Inspector ����
    [SerializeField] private Transform[] groundChecks;

    //Inspector ���� ����
    [Header("�׶��� üũ��")]
    [SerializeField] private float intoGroundDelay = 0.2f;
    [SerializeField] private float rayLengthForGroundCheck = 0.1f;
    [SerializeField] private float rayLengthForChromaDash = 0.25f;
    [Header("������")]
    [SerializeField] private float runSpeed = 5.0f;
    [SerializeField] private float hurtSlowSpeed = 2.0f;
    [SerializeField] private float chromaDashForce = 5.0f;
    [SerializeField] private float jumpForceFirst = 5.0f;
    [SerializeField] private float jumpForceContinue = 3.0f;
    [SerializeField] private float jumpForceHover = 1.0f;
    [SerializeField] private float jumpFirceContinueDuration = 1.2f;
    [Header("ũ�θ��뽬 ����")]
    [SerializeField] private float chromaDashTime = 1.0f;
    [SerializeField] private float chromaToRunTransitionTValue = 5.0f;
    [SerializeField] private float hurtToRunTransitionTValue = 2.0f;
    [Header("������ ����")]
    [SerializeField] private float chromaDashSuccessRewardAmount = 5.0f;
    [SerializeField] private float penaltyColorNoMatchAmount = 10.0f;
    [Header("�̺�Ʈ ����")]
    [SerializeField] private FloatEventChannelSO onPenaltyWhenNoColorMatch;

    //�ν����� �������� Getter
    public FloatEventChannelSO OnPenaltyWhenNoColorMatch { get { return onPenaltyWhenNoColorMatch; } }
    public float ChromaDashSuccesRewardAmount { get { return chromaDashSuccessRewardAmount; } }
    public float PenaltyColorNoMatchAmount { get { return penaltyColorNoMatchAmount; } }
    public float HurtSlowSpeed { get { return hurtSlowSpeed; } }
    public float ChromaDashForce { get { return chromaDashForce; } }
    public float JumpForceFirst { get { return jumpForceFirst; } }
    public float JumpForceContinue { get { return jumpForceContinue; } }
    public float JumpForceHover { get { return jumpForceHover; } }
    public float JumpFirceContinueDuration { get { return jumpFirceContinueDuration; } }
    public float ChromaDashTime { get { return chromaDashTime; } }

    //���
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

    private void Awake()
    {
        //������Ʈ
        Move = GetComponent<PlayerMove>();
        Anim = GetComponent<PlayerAnim>();
        //��ǲ�Ŵ��� ����ؼ� ���

        //�ʱ� ����
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

        //�̺�Ʈ
    }
    void Update()
    {
        GroundCheck();
        StateMachine.Update();
        if (Input.IsColorChangeLeftPressed) ChangeColorAsKeyLeft();
        if (Input.IsColorChangeRightPressed) ChangeColorAsKeyRight();


        //test�� reLoad
        if (Input.IsReLoadPressed) SceneManager.LoadScene(0);
    }
    private void FixedUpdate()
    {
        StateMachine.FixedUpdate();
        RunSpeedControl();
    }
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
        StateMachine.AddTransition(chromaDashState, jumpInAirState, () => Input.IsJumpPressed && !IsGround);
        StateMachine.AddTransition(hurtState, airState, () => hurtState.IsHurtEnd);

        //Any
        StateMachine.AddAnyTransition(hurtState,
            () => IsDelayedGround && ECurrentColor != EDetectedColorFromSeg && !(StateMachine.CurrentState is HurtState));
    }
    #region Transition Method
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
    IEnumerator IntoGroundDelay(float t)
    {
        yield return new WaitForSeconds(t);
        IsDelayedGround = true;
    }
    #region ������
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
            //���� ���� �ʱ�ȭ!
            WasJumpedInAir = false;
            WasSecondJumpedInAir = false;
        }
        if (IsDelayedGround)
        {
            //���� ���� �ʱ�ȭ!
            WasJumpedInAir = false;
            WasSecondJumpedInAir = false;
            //ũ�θ� ������ �ʱ�ȭ
            IsInChromaDashDistance = false;
        }
    }
    //AirState FixedUpdate()���� �˻�
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
            else
            {
                EDetectedColorFromSeg = EChromaColor.None;
            }
        }
        IsInChromaDashDistance = result;
    }
    #endregion

    #region �÷��̾� ���󺯰� - ��𼭵� ������ ���� PlayerController���� ����
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

    #region ������
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
