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
    public bool IsPausePressed { get; private set; }
    public bool IsReLoadRPressed { get; private set; }
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
        IsQuickSlot1Pressed = Input.GetKeyDown(KeyCode.Alpha1);
        IsQuickSlot2Pressed = Input.GetKeyDown(KeyCode.Alpha2);
        IsQuickSlot3Pressed = Input.GetKeyDown(KeyCode.Alpha3);

        IsReLoadRPressed = Input.GetKeyDown(KeyCode.R);
    }
}
