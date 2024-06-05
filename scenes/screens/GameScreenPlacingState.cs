using Godot;
using System;

public partial class GameScreenPlacingState : GameScreenState
{
    override public async void OnEnter()
    {
        BlockBoard.ClearPreview();
        var task = BlockBoard.ResolveBoardForNewPolyomino(CurPolyomino, GameScreen.LeftIndex);
        CurPolyomino.BlockGrid.UpdateBlocksFromTypes(new BlockType?[0, 0]);
        await task;
        bool isValid = !GameScreen.HasOverflowBlocks();

        if (isValid)
        {
            GameScreen.AdvancePolyomino();
            GameScreen.WallKick();
            BlockBoard.UpdatePreviewPolyomino(CurPolyomino, GameScreen.LeftIndex);
            EmitSignal(SignalName.Transitioned, "GameScreenInputState");
        }
        else
        {
            EmitSignal(SignalName.Transitioned, "GameScreenGameOverState");
        }
    }

    override public void OnExit() { }
}
