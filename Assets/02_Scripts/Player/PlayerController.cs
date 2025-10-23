using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //component
    public PlayerMove Move { get; private set; }
    public PlayerAnim Anim { get; private set; }

    //state
    public StateMachine StateMachine { get; private set; }

    //input
    public InputManager Input { get; private set; }
    private bool isJump;
    private void Awake()
    {
        Move = GetComponent<PlayerMove>();
        Anim = GetComponent<PlayerAnim>();
        Input = Managers.Input;

        StateMachine = new StateMachine();
    }
    private void Start()
    {
        Anim.PlayAnim(AnimHash.fastRunHash);
    }
    void Update()
    {
        if (Input.IsJumpPressed) isJump = true;
    }
    private void FixedUpdate()
    {
        if (isJump)
        {
            isJump = false;
            Move.AddForceImpulseY(2.0f);
        }
    }
}
