using Godot;
using System;

abstract public partial class PlayerState : Node
{
    [Signal]
    public delegate void TransitionedEventHandler(String newStateName);

    [Export]
    protected GameSession GameSession { get; private set; }
    protected BlockBoard BlockBoard { get { return GameSession.BlockBoard; } }
    protected Polyomino CurPolyomino { get { return GameSession.CurPolyomino; } }

    abstract public void OnEnter();
    abstract public void OnExit();
}
