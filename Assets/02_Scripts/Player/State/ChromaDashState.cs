using System.Collections;
using UnityEngine;

public class ChromaDashState : PlayerState
{
    public ChromaDashState(PlayerController player, IState parent = null) : base(player, parent) { }
    private Coroutine dashTimeCo;
    public override void Enter()
    {
        player.IsChromaDash = true;

        player.Move.AddForceImpulseX(player.ChromaDashForce);
        player.Anim.PlayAnim(AnimHash.fastRunHash);
        dashTimeCo = player.StartCoroutine(ChromaDashTimeCo(player.ChromaDashTime));
        base.Enter();
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
        player.StopCoroutine(dashTimeCo);
        player.IsChromaDash = false;
    }
    IEnumerator ChromaDashTimeCo(float t)
    {
        yield return new WaitForSeconds(t);
        player.IsChromaDash = false;
    }
}
