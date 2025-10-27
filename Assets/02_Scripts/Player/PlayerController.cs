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
    public GroundState GroundState { get; private set; }
    public JumpOnGroundState JumpOnGroundState { get; private set; }
    public AirState AirState { get; private set; }
    public JumpInAirState JumpInAirState { get; private set; }

    //input
    public InputManager Input { get; private set; }

    //Inspector 참조
    [SerializeField] private Transform[] groundChecks;

    //Inspector 조절 변수
    [Header("GroundCheck")]
    [SerializeField] private float rayLengthForGroundCheck = 0.1f;
    [SerializeField] private float rayLengthForChromaDash = 0.25f;
    [Header("물리값")]
    [SerializeField] private float runSpeed = 5.0f;
    [SerializeField] private float chromaDashForce = 5.0f;
    [SerializeField] private float chromaDashDruration = 1.0f;
    [SerializeField] private float jumpForceFirst = 5.0f;
    [SerializeField] private float jumpForceContinue = 3.0f;
    [SerializeField] private float jumpForceHover = 1.0f;
    [SerializeField] private float jumpFirceContinueDuration = 1.2f;

    //인스펙터 조절변수 Getter
    public float RunSpeed { get { return runSpeed; } }
    public float ChromaDashForce { get { return chromaDashForce; } }
    public float ChromaDashDruration { get { return chromaDashDruration; } }
    public float JumpForceFirst { get { return jumpForceFirst; } }
    public float JumpForceContinue { get { return jumpForceContinue; } }
    public float JumpForceHover { get { return jumpForceHover; } }
    public float JumpFirceContinueDuration { get { return jumpFirceContinueDuration; } }

    //멤버
    public bool IsGround { get; private set; }
    public bool CanChromaDashDistance { get; private set; }
    public bool IsChromaDash { get; private set; } = false;
    public bool WasJumpedOnGround { get; set; } = false;
    public bool WasJumpedInAir { get; private set; } = false;
    public bool WasSecondJumpedInAir { get; private set; } = false;

    private EChromaColor eCurrentColor = EChromaColor.Red;
    private void Awake()
    {
        //컴포넌트
        Move = GetComponent<PlayerMove>();
        Anim = GetComponent<PlayerAnim>();
        //인풋매니저 등록해서 사용
        Input = Managers.Input;

        //초기 색상
        eCurrentColor = EChromaColor.Red;

        //States
        StateMachine = new StateMachine();
        GroundState = new GroundState(this);
        JumpOnGroundState = new JumpOnGroundState(this);
        AirState = new AirState(this);
        JumpInAirState = new JumpInAirState(this, AirState);
        AddTransitions();

        //
    }
    private void Start()
    {
        StateMachine.ChangeState(AirState);
        Move.SetVelocityX(runSpeed);
    }
    void Update()
    {
        GroundCheck();
        StateMachine.Update();
        if (Input.IsColorChangeLeftPressed) ChangeColorAsKeyLeft();
        if (Input.IsColorChangeRightPressed) ChangeColorAsKeyRight();
        if (Input.IsJumpPressed && IsGround) WasJumpedOnGround = true;

        //test용 reLoad
        if (Input.IsReLoadRPressed) SceneManager.LoadScene(0);
    }
    private void FixedUpdate()
    {
        StateMachine.FixedUpdate();
        RunSpeedControl();
    }
    private void AddTransitions()
    {
        StateMachine.AddTransition(AirState, GroundState, () => IsGround);
        StateMachine.AddTransition(GroundState, AirState, () => !IsGround);
        StateMachine.AddTransition(GroundState, JumpOnGroundState, () => Input.IsJumpPressed);
        StateMachine.AddTransition(JumpOnGroundState, AirState, () => JumpOnGroundState.DoChangeStateJumpOnGroundToAirIdle);
        StateMachine.AddTransition(JumpOnGroundState, JumpInAirState, new Func<bool>(DoChangeStateJumpOnGroundToJump));
        StateMachine.AddTransition(JumpInAirState, AirState, () => JumpInAirState.DoChangeStateJumpInAirToAirIdle);
        StateMachine.AddTransition(AirState, JumpInAirState, new Func<bool>(DoChangeStateAirToJump));


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
                Debug.Log("공중 2단 점프!");
                WasSecondJumpedInAir = true;
                return true;
            }
            else return false;
        }
    }

    #endregion
    private void RunSpeedControl()
    {
        if (!IsChromaDash)
        {
            Move.SetVelocityX(runSpeed);
        }
        if (IsChromaDash)
        {
            IsChromaDash = false;
            Move.AddForceImpulseX(chromaDashForce);
        }
    }

    #region 감지들
    private void GroundCheck()
    {
        bool[] isGrounds = new bool[groundChecks.Length];
        bool result = false;
        for (int i = 0; i < groundChecks.Length; i++)
        {
            isGrounds[i] = false;
            result = false;
            RaycastHit2D col = Physics2D.Raycast(groundChecks[i].position, Vector2.down, rayLengthForGroundCheck,
                LayerManager.GetLayerMask(ELayerName.Ground));
            if (col.collider != null) isGrounds[i] = true;
            else isGrounds[i] = false;
            result |= isGrounds[i];
        }
        IsGround = result;
        if (IsGround)
        {
            //점프 조건 초기화!
            WasJumpedInAir = false;
            WasSecondJumpedInAir = false;
        }
    }
    public void ChromaDashCheck()
    {
        bool[] isGrounds = new bool[groundChecks.Length];
        bool result = false;
        for (int i = 0; i < groundChecks.Length; i++)
        {
            isGrounds[i] = false;
            result = false;
            RaycastHit2D col = Physics2D.Raycast(groundChecks[i].position, Vector2.down, rayLengthForChromaDash,
                LayerManager.GetLayerMask(ELayerName.Ground));
            if (col.collider != null) isGrounds[i] = true;
            else isGrounds[i] = false;
            result |= isGrounds[i];
        }
        CanChromaDashDistance = result;
    }
    #endregion

    #region 플레이어 색상변경 - 어디서든 적용을 위해 PlayerController에서 변경
    private void ChangeColorAsKeyLeft()
    {
        switch (eCurrentColor)
        {
            case EChromaColor.Red:
                Anim.ChangeColor(EChromaColor.Green);
                eCurrentColor = EChromaColor.Green;
                break;
            case EChromaColor.Blue:
                Anim.ChangeColor(EChromaColor.Red);
                eCurrentColor = EChromaColor.Red;
                break;
            case EChromaColor.Green:
                Anim.ChangeColor(EChromaColor.Blue);
                eCurrentColor = EChromaColor.Blue;
                break;
            default:
                Anim.ChangeColor(EChromaColor.Red);
                eCurrentColor = EChromaColor.Red;
                break;
        }
    }
    private void ChangeColorAsKeyRight()
    {
        switch (eCurrentColor)
        {
            case EChromaColor.Red:
                Anim.ChangeColor(EChromaColor.Blue);
                eCurrentColor = EChromaColor.Blue;
                break;
            case EChromaColor.Blue:
                Anim.ChangeColor(EChromaColor.Green);
                eCurrentColor = EChromaColor.Green;
                break;
            case EChromaColor.Green:
                Anim.ChangeColor(EChromaColor.Red);
                eCurrentColor = EChromaColor.Red;
                break;
            default:
                Anim.ChangeColor(EChromaColor.Red);
                eCurrentColor = EChromaColor.Red;
                break;
        }
    }
    #endregion

    #region 기즈모들
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        foreach (Transform groundCheck in groundChecks)
        {
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + new Vector3(0, -rayLengthForChromaDash, 0));
            RaycastHit2D col = Physics2D.Raycast(groundCheck.position, Vector2.down, rayLengthForChromaDash,
                LayerManager.GetLayerMask(ELayerName.Ground));
            if (col.collider != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(groundCheck.position, groundCheck.position + new Vector3(0, -rayLengthForChromaDash, 0));
            }
        }
        Gizmos.color = Color.yellow;
        foreach (Transform groundCheck in groundChecks)
        {
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + new Vector3(0, -rayLengthForGroundCheck, 0));
            RaycastHit2D col = Physics2D.Raycast(groundCheck.position, Vector2.down, rayLengthForGroundCheck,
                LayerManager.GetLayerMask(ELayerName.Ground));
            if (col.collider != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(groundCheck.position, groundCheck.position + new Vector3(0, -rayLengthForGroundCheck, 0));
            }
        }
    }
    #endregion
}
