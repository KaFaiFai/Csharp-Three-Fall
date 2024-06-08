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
    }

    public void GameStarted()
    {
        _playerHand.WallKick(_blockBoard);
        _blockBoard.UpdatePreviewPolyomino(_playerHand.CurPolyomino, _playerHand.LeftIndex);
        EmitSignal(SignalName.Transitioned, _nextInputState);
    }

    override public void OnExit()
    {
        _inputEvents.ConfirmPressed -= GameStarted;
    }
}
