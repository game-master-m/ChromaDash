using UnityEngine;

public class ReadyChromaDashState : PlayerState
{
    public ReadyChromaDashState(PlayerController player, IState parent = null) : base(player, parent) { }

    public int ComboBonus { get; private set; } = 0;
    public override void Enter()
    {
        base.Enter();
        ComboBonus = 0;
    }
    public override void Update()
    {
        base.Update();
        if (Managers.Input.IsColorChangeLeftPressed) ComboBonus++;
        if (Managers.Input.IsColorChangeRightPressed) ComboBonus++;
        player.CanChromaDashReady = (player.ECurrentColor == player.EDetectedColorFromSeg) ? true : false;
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
