using Godot;
using System;

public partial class PlayerGameOverState : PlayerState
{
    override public void OnEnter()
    {
        GD.Print("Has lost");
    }

    override public void OnExit()
    {
    }
}
