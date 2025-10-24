using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //component
    public PlayerMove Move { get; private set; }
    public PlayerAnim Anim { get; private set; }

    //state
    public StateMachine StateMachine { get; private set; }
    public GroundState GroundState { get; private set; }
    public AirState AirState { get; private set; }
    public JumpInAirState JumpInAirState { get; private set; }
    public JumpOnGroundState JumpOnGroundState { get; private set; }

    //input
    public InputManager Input { get; private set; }

    //Inspector 참조
    [SerializeField] private Transform[] groundChecks;

    //Inspector 조절 변수
    [Header("GroundCheck")]
    [SerializeField] float rayLengthForGroundCheck = 0.1f;
    [SerializeField] float rayLengthForChromaDash = 0.25f;
    [Header("물리값")]
    [SerializeField] public float jumpForceFirst = 5.0f;
    [SerializeField] public float jumpForceContinue = 3.0f;
    [SerializeField] public float jumpForceHover = 1.0f;
    [SerializeField] public float jumpFirceContinueDuration = 1.2f;


    //멤버
    private EChromaColor currentColor = EChromaColor.Red;
    private bool isJump;
    public bool IsGround { get; private set; }
    public bool IsChromaDash { get; private set; }
    private void Awake()
    {
        //컴포넌트
        Move = GetComponent<PlayerMove>();
        Anim = GetComponent<PlayerAnim>();
        //인풋매니저 등록해서 사용
        Input = Managers.Input;

        //초기 색상
        currentColor = EChromaColor.Red;

        //States
        StateMachine = new StateMachine();
        GroundState = new GroundState(this);
        AirState = new AirState(this);
        JumpInAirState = new JumpInAirState(this, AirState);
        JumpOnGroundState = new JumpOnGroundState(this);
        AddTransitions();

        //
    }
    private void Start()
    {
        StateMachine.ChangeState(AirState);
    }
    void Update()
    {
        GroundCheck();
        StateMachine.Update();
        if (Input.IsColorChangeLeftPressed) ChangeColorAsKeyLeft();
        if (Input.IsColorChangeRightPressed) ChangeColorAsKeyRight();
    }
    private void FixedUpdate()
    {
        StateMachine.FixedUpdate();
    }
    #region 감지들
    private void AddTransitions()
    {
        StateMachine.AddTransition(AirState, GroundState, () => IsGround);
        StateMachine.AddTransition(GroundState, AirState, () => !IsGround);
        StateMachine.AddTransition(GroundState, JumpOnGroundState, () => Input.IsJumpPressed);
        StateMachine.AddTransition(JumpOnGroundState, AirState, () => JumpOnGroundState.IsChangeToAirState);
    }


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
        IsChromaDash = result;
    }
    #endregion

    #region 플레이어 색상변경 - 어디서든 적용을 위해 PlayerController에서 변경
    private void ChangeColorAsKeyLeft()
    {
        switch (currentColor)
        {
            case EChromaColor.Red:
                Anim.ChangeColor(EChromaColor.Green);
                currentColor = EChromaColor.Green;
                break;
            case EChromaColor.Blue:
                Anim.ChangeColor(EChromaColor.Red);
                currentColor = EChromaColor.Red;
                break;
            case EChromaColor.Green:
                Anim.ChangeColor(EChromaColor.Blue);
                currentColor = EChromaColor.Blue;
                break;
            default:
                Anim.ChangeColor(EChromaColor.Red);
                currentColor = EChromaColor.Red;
                break;
        }
    }
    private void ChangeColorAsKeyRight()
    {
        switch (currentColor)
        {
            case EChromaColor.Red:
                Anim.ChangeColor(EChromaColor.Blue);
                currentColor = EChromaColor.Blue;
                break;
            case EChromaColor.Blue:
                Anim.ChangeColor(EChromaColor.Green);
                currentColor = EChromaColor.Green;
                break;
            case EChromaColor.Green:
                Anim.ChangeColor(EChromaColor.Red);
                currentColor = EChromaColor.Red;
                break;
            default:
                Anim.ChangeColor(EChromaColor.Red);
                currentColor = EChromaColor.Red;
                break;
        }
    }
    #endregion

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
}
