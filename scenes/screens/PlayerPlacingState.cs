using Godot;
using System;

public partial class PlayerPlacingState : PlayerState
{
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
            EmitSignal(SignalName.Transitioned, "PlayerInputState");
        }
        else
        {
            EmitSignal(SignalName.Transitioned, "PlayerGameOverState");
        }
    }

    override public void OnExit() { }
}
