public interface IState
{
    void Enter();
    void Update();
    void FixedUpdate();
    void Exit();
}
public abstract class PlayerState : IState
{
    protected readonly PlayerController player;
    protected float elapsedTimeBase = 0f;
    public IState Parent { get; }

    public PlayerState(PlayerController player, IState parent = null)
    {
        this.player = player;
        Parent = parent;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }

}
