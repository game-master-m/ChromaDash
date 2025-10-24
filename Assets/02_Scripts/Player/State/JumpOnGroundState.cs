using System.Collections;
using UnityEngine;

public class JumpOnGroundState : PlayerState
{
    public JumpOnGroundState(PlayerController player, PlayerState parent = null) : base(player, parent) { }

    private float clipLength = 0.2f;
    private bool isEndClip = false;
    private Coroutine delayCo;

    private float elapsedTime = 0.0f;
    private bool isPressing = false;
    private bool isPressingEnd = false;
    public bool IsChangeToAirState { get; private set; } = false;
    public override void Enter()
    {
        base.Enter();
        player.Anim.PlayAnim(AnimHash.jumpOnGroundHash);
        player.StartCoroutine(DelayForCollisionComplexCo());
        player.Move.SetVelocityY(0.0f);
        player.Move.AddForceImpulseY(player.jumpForceFirst);
        delayCo = player.StartCoroutine(DelayClipLengthCo());
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
            IsChangeToAirState = true;
        }

    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (isPressing && !isPressingEnd)
        {
            player.Move.AddForceLerpYEaseIn(player.jumpForceContinue,
                player.jumpForceHover, player.jumpFirceContinueDuration, elapsedTime);
        }
    }
    public override void Exit()
    {
        base.Exit();
        isEndClip = false;
        isPressing = false;
        player.StopCoroutine(delayCo);
        IsChangeToAirState = false;
        isPressingEnd = false;
        elapsedTime = 0.0f;
    }
    IEnumerator DelayClipLengthCo()
    {
        yield return new WaitForSeconds(clipLength);
        isEndClip = true;
        yield return new WaitForSeconds(player.jumpFirceContinueDuration - clipLength);
        IsChangeToAirState = true;
    }
    IEnumerator DelayForCollisionComplexCo()
    {
        yield return new WaitForSeconds(0.1f);
    }
}
