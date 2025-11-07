using TMPro;
using UnityEngine;

public class ReadyChromaDashState : PlayerState
{
    public ReadyChromaDashState(PlayerController player, IState parent = null) : base(player, parent) { }
    public bool CanChromaDashReady { get; set; } = false;
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
        CanChromaDashReady = (player.ECurrentColor == player.EDetectedColorFromSeg && player.IsInChromaDashDistance) ? true : false;
    }
    public override void FixedUpdate()
    {
        Parent.FixedUpdate();
    }
    public override void Exit()
    {
        base.Exit();
        CanChromaDashReady = false;
    }
}
