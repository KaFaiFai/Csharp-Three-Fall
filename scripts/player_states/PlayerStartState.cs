using Godot;
using System;

public partial class PlayerStartState : PlayerState
{
    [Export]
    private PlayerInput _inputEvents;

    [Export]
    private PlayerState _nextInputState;

    override public void OnEnter()
    {
        _inputEvents.ConfirmPressed += GameStarted;
        BlockBoard?.ClearGridLines();
    }

    public void GameStarted()
    {
        GameSession.Rng = new RandomNumberGenerator() { Seed = 0 };
        GameSession.LeftIndex = 0;

        GameSession.AdvancePolyomino();
        GameSession.AdvancePolyomino();
        BlockBoard.BlockGrid.UpdateBlocksFromTypes(new BlockType?[GameSession.BoardSize.X, GameSession.BoardSize.Y]);
        GameSession.WallKick();
        BlockBoard.UpdatePreviewPolyomino(CurPolyomino, GameSession.LeftIndex);
        BlockBoard.DrawGridLines(GameSession.OverflowFrom + 1);
        EmitSignal(SignalName.Transitioned, _nextInputState);
    }

    override public void OnExit()
    {
        _inputEvents.ConfirmPressed -= GameStarted;
    }


}