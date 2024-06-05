using Godot;
using System;

abstract public partial class GameScreenState : Node
{
    [Signal]
    public delegate void TransitionedEventHandler(String newStateName);

    [Export]
    protected GameScreen GameScreen { get; private set; }
    protected BlockBoard BlockBoard { get { return GameScreen.BlockBoard; } }
    protected Polyomino CurPolyomino { get { return GameScreen.CurPolyomino; } }

    abstract public void OnEnter();
    abstract public void OnExit();
}
