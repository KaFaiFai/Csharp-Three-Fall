using Godot;
using System;

public partial class GameScreenGameOverState : GameScreenState
{
    override public void OnEnter()
    {
        GD.Print("Has lost");
    }

    override public void OnExit()
    {
    }
}
