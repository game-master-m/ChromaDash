using UnityEngine;

public class JumpInAirState : PlayerState
{
    public JumpInAirState(PlayerController player, PlayerState parent = null) : base(player, parent) { }

    public override void Enter()
    {
        base.Enter();

    }
    public override void Update()
    {
        Parent.Update();

    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    public override void Exit()
    {
        base.Exit();
    }
}
