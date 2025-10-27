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
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        player.ChromaDashCheck();

        player.Anim.SetFloat(AnimHash.velocityYHash, player.Move.GetVelocityY());
        player.Anim.SetBool(AnimHash.canChromaDashDistanceHash, player.IsInChromaDashDistance);
    }
    public override void Exit()
    {
        base.Exit();
        if (player.IsGround) player.WasJumpedOnGround = false;
    }
}
