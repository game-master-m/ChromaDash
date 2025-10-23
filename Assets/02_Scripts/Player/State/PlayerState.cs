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
    public PlayerState Parent { get; }

    public PlayerState(PlayerController player, PlayerState parent = null)
    {
        this.player = player;
        Parent = parent;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }

}
