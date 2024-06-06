using Godot;
using System;

public partial class PlayerPlacingState : PlayerState
{
    [Export]
    private PlayerState _nextInputState;
    private PlayerState _nextGameOverState;

    override public async void OnEnter()
    {
        BlockBoard.ClearPreview();
        var task = BlockBoard.ResolveBoardForNewPolyomino(CurPolyomino, GameSession.LeftIndex);
        CurPolyomino.BlockGrid.UpdateBlocksFromTypes(new BlockType?[0, 0]);
        await task;
        bool isValid = !GameSession.HasOverflowBlocks();

        if (isValid)
        {
            GameSession.AdvancePolyomino();
            GameSession.WallKick();
            BlockBoard.UpdatePreviewPolyomino(CurPolyomino, GameSession.LeftIndex);
            EmitSignal(SignalName.Transitioned, _nextInputState);
        }
        else
        {
            EmitSignal(SignalName.Transitioned, _nextGameOverState);
        }
    }

    override public void OnExit() { }
}
