using Godot;
using System;

public partial class PlayerGameOverState : PlayerState
{
    [Export] private Control GameOver;

    override public void OnEnter()
    {
        GameOver.Visible = true;
    }

    override public void OnExit()
    {
        GameOver.Visible = false;
    }
}
