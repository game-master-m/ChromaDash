using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtState : PlayerState
{
    public HurtState(PlayerController player, IState parent = null) : base(player, parent) { }

    private float clipLength = 0.63f;
    public bool IsHurtEnd { get; private set; } = false;
    public bool IsHurtSlow { get; set; }
    public override void Enter()
    {
        base.Enter();
        Debug.Log("Hurt State Enter");
        player.Anim.CrossFade(AnimHash.hurtHash, 0.1f);
        player.Move.SetVelocityX(player.HurtSlowSpeed);
        IsHurtSlow = true;
        player.StartCoroutine(DelayHurtMotionCo(clipLength));

        player.OnPenaltyWhenNoColorMatch.Rasied(player.PenaltyColorNoMatchAmount);

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
        IsHurtEnd = false;

    }
    IEnumerator DelayHurtMotionCo(float t)
    {
        yield return new WaitForSeconds(t);
        IsHurtEnd = true;
    }
}
