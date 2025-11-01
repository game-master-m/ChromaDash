using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public bool IsJumpPressed { get; private set; }
    public bool IsJumpPressing { get; private set; }
    public bool IsColorChangeLeftPressed { get; private set; }
    public bool IsColorChangeRightPressed { get; private set; }
    public bool IsQuickSlot1Pressed { get; private set; }
    public bool IsQuickSlot2Pressed { get; private set; }
    public bool IsQuickSlot3Pressed { get; private set; }
    public bool IsReLoadPressed { get; private set; }

    //�̺�Ʈ
    [Header("�̺�Ʈ ����")]
    [SerializeField] private IntEventChannelSO onUseQuickSlotRequest;
    [SerializeField] private VoidEventChannelSO onPauseRequest;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        IsJumpPressed = Input.GetKeyDown(KeyCode.Space);
        IsJumpPressing = Input.GetKey(KeyCode.Space);
        IsColorChangeLeftPressed = Input.GetKeyDown(KeyCode.LeftArrow);
        IsColorChangeRightPressed = Input.GetKeyDown(KeyCode.RightArrow);

        if (Input.GetKeyDown(KeyCode.Alpha1)) onUseQuickSlotRequest?.Raised(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) onUseQuickSlotRequest?.Raised(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) onUseQuickSlotRequest?.Raised(2);

        if (Input.GetKeyDown(KeyCode.Escape)) onPauseRequest?.Raised();

        IsReLoadPressed = Input.GetKeyDown(KeyCode.R);
    }
}
