using Godot;
using System;

abstract public partial class PlayerState : Node
{
    [Signal]
    public delegate void TransitionedEventHandler(PlayerState newState);

    abstract public void OnEnter();
    abstract public void OnExit();
}
