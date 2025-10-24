using UnityEngine;

public class AirState : PlayerState
{
    public AirState(PlayerController player, PlayerState parent = null) : base(player, parent) { }

    public override void Enter()
    {
        base.Enter();
        player.Anim.CrossFade(AnimHash.airIdledHash, 0.2f);
    }
    public override void Update()
    {
        base.Update();
        player.ChromaDashCheck();
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
