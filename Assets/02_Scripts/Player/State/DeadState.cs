using UnityEngine;

public class DeadState : PlayerState
{
    public DeadState(PlayerController player, IState parent = null) : base(player, parent) { }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Dead State Enter");
        player.Anim.CrossFade(AnimHash.deadHash, 0.3f);
    }
}
