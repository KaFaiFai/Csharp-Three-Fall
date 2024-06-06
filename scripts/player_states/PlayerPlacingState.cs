using Godot;
using System;

public partial class PlayerPlacingState : PlayerState
{
    [Export] private PlayerState _nextInputState;
    [Export] private PlayerState _nextGameOverState;

    [Export] private BlockBoard _blockBoard;
    [Export] private PlayerHand _playerHand;

    override public async void OnEnter()
    {
        _blockBoard.ClearPreview();
        var task = _blockBoard.ResolveBoardForNewPolyomino(_playerHand.CurPolyomino, _playerHand.LeftIndex);
        _playerHand.CurPolyomino.BlockGrid.UpdateBlocksFromTypes(new BlockType?[0, 0]);
        await task;
        bool isValid = !_blockBoard.HasOverflowBlocks();

        if (isValid)
        {
            _playerHand.AdvancePolyomino();
            _playerHand.WallKick(_blockBoard);
            _blockBoard.UpdatePreviewPolyomino(_playerHand.CurPolyomino, _playerHand.LeftIndex);
            EmitSignal(SignalName.Transitioned, _nextInputState);
        }
        else
        {
            EmitSignal(SignalName.Transitioned, _nextGameOverState);
        }
    }

    override public void OnExit() { }
}
