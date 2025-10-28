using System.Collections;
using UnityEngine;

public class JumpOnGroundState : PlayerState
{
    public JumpOnGroundState(PlayerController player, PlayerState parent = null) : base(player, parent) { }

    private float clipLength = 0.3f;
    private bool isEndClip = false;
    private Coroutine delayClipLenthCo;
    private Coroutine delayForSafeCo;

    private float elapsedTime = 0.0f;
    private bool isPressing = false;
    private bool isPressingEnd = false;

    public bool DoChangeStateJumpOnGroundToAirIdle { get; private set; } = false;
    public override void Enter()
    {
        Debug.Log("Jumped On Ground State Enter");
        base.Enter();
        player.WasJumpedOnGround = true;
        player.Anim.PlayAnim(AnimHash.jumpOnGroundHash);
        delayForSafeCo = player.StartCoroutine(DelayForCollisionComplexCo());
        player.Move.SetVelocityY(0.0f);
        player.Move.AddForceImpulseY(player.JumpForceFirst);
        delayClipLenthCo = player.StartCoroutine(DelayClipLengthCo());


    }
    public override void Update()
    {
        base.Update();
        if (Managers.Input.IsJumpPressing)
        {
            isPressing = true;
        }
        else
        {
            isPressing = false;
            isPressingEnd = true;
        }
        if (!Managers.Input.IsJumpPressing && isEndClip || isEndClip && isPressingEnd)
        {
            DoChangeStateJumpOnGroundToAirIdle = true;
        }

    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (isPressing && !isPressingEnd)
        {
            player.Move.AddForceYLerpYEaseIn(player.JumpForceContinue,
                player.JumpForceHover, player.JumpFirceContinueDuration, elapsedTime);
        }
    }
    public override void Exit()
    {
        base.Exit();
        isEndClip = false;
        isPressing = false;
        player.StopCoroutine(delayClipLenthCo);
        player.StopCoroutine(delayForSafeCo);
        DoChangeStateJumpOnGroundToAirIdle = false;
        isPressingEnd = false;
        elapsedTime = 0.0f;
    }
    IEnumerator DelayClipLengthCo()
    {
        yield return new WaitForSeconds(clipLength);
        isEndClip = true;
        yield return new WaitForSeconds(player.JumpFirceContinueDuration - clipLength);
        DoChangeStateJumpOnGroundToAirIdle = true;
    }
    IEnumerator DelayForCollisionComplexCo()
    {
        yield return new WaitForSeconds(0.1f);
    }
}
