using System.Collections;
using UnityEngine;

public class JumpInAirState : PlayerState
{
    public JumpInAirState(PlayerController player, PlayerState parent = null) : base(player, parent) { }

    private float clipLength = 0.3f;
    private bool isEndClip = false;
    private Coroutine delayClipLengthCo;
    private Coroutine delayForSafeCo;

    private float elapsedTime = 0.0f;
    private bool isPressing = false;
    private bool isPressingEnd = false;
    //private bool canJumpAgain = true;
    public bool DoChangeStateJumpInAirToAirIdle { get; private set; } = false;

    private float velocityRateWhenJumpPressingEnd = 0.85f;
    public override void Enter()
    {
        base.Enter();
        Managers.Sound.PlaySFX(ESfxName.Jump);
        player.Anim.CrossFade(AnimHash.jumpOnGroundHash, 0.1f);
        delayForSafeCo = player.StartCoroutine(DelayForCollisionComplexCo());
        player.Move.SetVelocityY(0.0f);
        player.Move.AddForceImpulseY(player.JumpForceFirst);
        delayClipLengthCo = player.StartCoroutine(DelayClipLengthCo());
        player.OnJumpSuccess.Raised();
    }
    public override void Update()
    {
        base.Update();

        if (Managers.Input.IsJumpPressing && !isPressingEnd)
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
            DoChangeStateJumpInAirToAirIdle = true;
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
        else
        {
            player.Move.SetVelocityY(player.Move.GetVelocityY() * velocityRateWhenJumpPressingEnd);
        }
    }
    public override void Exit()
    {
        base.Exit();
        isEndClip = false;
        isPressing = false;
        player.StopCoroutine(delayClipLengthCo);
        player.StopCoroutine(delayForSafeCo);
        DoChangeStateJumpInAirToAirIdle = false;
        isPressingEnd = false;
        elapsedTime = 0.0f;

        player.Move.SetVelocityY(player.Move.GetVelocityY() * velocityRateWhenJumpPressingEnd);
    }
    IEnumerator DelayClipLengthCo()
    {
        yield return new WaitForSeconds(clipLength);
        isEndClip = true;
        yield return new WaitForSeconds(player.JumpFirceContinueDuration - clipLength);
        DoChangeStateJumpInAirToAirIdle = true;
    }
    IEnumerator DelayForCollisionComplexCo()
    {
        yield return new WaitForSeconds(0.1f);
    }
}
