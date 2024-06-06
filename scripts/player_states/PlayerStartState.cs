using Godot;
using System;

public partial class PlayerStartState : PlayerState
{
    [Export] private PlayerState _nextInputState;

    [Export] private PlayerInput _inputEvents;
    [Export] private BlockBoard _blockBoard;
    [Export] private PlayerHand _playerHand;

    override public void OnEnter()
    {
        _inputEvents.ConfirmPressed += GameStarted;
        _blockBoard?.ClearGridLines();
    }

    public void GameStarted()
    {
        _playerHand.LeftIndex = 0;
        _playerHand.AdvancePolyomino();
        _playerHand.AdvancePolyomino();

        BlockType?[,] newEmptyBoard = new BlockType?[_blockBoard.InitialBoardSize.X, _blockBoard.InitialBoardSize.Y];
        _blockBoard.BlockGrid.UpdateBlocksFromTypes(newEmptyBoard);
        _playerHand.WallKick(_blockBoard);
        _blockBoard.UpdatePreviewPolyomino(_playerHand.CurPolyomino, _playerHand.LeftIndex);
        _blockBoard.DrawGridLines();
        EmitSignal(SignalName.Transitioned, _nextInputState);
    }

    override public void OnExit()
    {
        _inputEvents.ConfirmPressed -= GameStarted;
    }
}
