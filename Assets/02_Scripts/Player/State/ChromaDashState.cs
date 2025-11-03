using System.Collections;
using UnityEngine;

public class ChromaDashState : PlayerState
{
    public ChromaDashState(PlayerController player, IState parent = null) : base(player, parent) { }
    private Coroutine dashTimeCo;
    public override void Enter()
    {
        base.Enter();
        player.IsChromaDash = true;
        player.WasJumpedOnGround = false;
        player.Move.AddForceImpulseX(player.ChromaDashForce);
        player.Anim.PlayAnim(AnimHash.fastRunHash);
        dashTimeCo = player.StartCoroutine(ChromaDashTimeCo(player.ChromaDashTime));
        player.OnChromaDashSuccess.Raised(player.ChromaDashSuccesRewardAmount);
    }
    public override void Update()
    {
        base.Update();
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        //player.Move.SetVelocityX(10.0f);
    }
    public override void Exit()
    {
        base.Exit();
        //player.StopCoroutine(dashTimeCo); 항상 제 시간동안 적용되게.
        //player.IsChromaDash = false;
    }
    IEnumerator ChromaDashTimeCo(float t)
    {
        yield return new WaitForSeconds(t);
        player.IsChromaDash = false;
    }
}
