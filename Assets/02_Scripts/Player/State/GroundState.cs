using UnityEngine;

public class GroundState : PlayerState
{
    public GroundState(PlayerController player, PlayerState parent = null) : base(player, parent) { }

    public override void Enter()
    {
        base.Enter();
        player.Anim.CrossFade(AnimHash.runHash, 0.0f);

    }
    public override void Update()
    {
        base.Update();
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
