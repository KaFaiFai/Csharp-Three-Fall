using Godot;
using System;

public partial class GameScreenStartState : GameScreenState
{
    [Export]
    private InputEvents _inputEvents;

    override public void OnEnter()
    {
        _inputEvents.ConfirmPressed += GameStarted;
        BlockBoard?.ClearGridLines();
    }

    public void GameStarted()
    {
        GameScreen.Rng = new RandomNumberGenerator() { Seed = 0 };
        GameScreen.LeftIndex = 0;

        GameScreen.AdvancePolyomino();
        GameScreen.AdvancePolyomino();
        BlockBoard.BlockGrid.UpdateBlocksFromTypes(new BlockType?[GameScreen.BoardSize.X, GameScreen.BoardSize.Y]);
        GameScreen.WallKick();
        BlockBoard.UpdatePreviewPolyomino(CurPolyomino, GameScreen.LeftIndex);
        BlockBoard.DrawGridLines(GameScreen.OverflowFrom + 1);
        EmitSignal(SignalName.Transitioned, "GameScreenInputState");
    }

    override public void OnExit()
    {
        _inputEvents.ConfirmPressed -= GameStarted;
    }


}
