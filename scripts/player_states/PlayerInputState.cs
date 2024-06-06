using Godot;
using System;

public partial class PlayerInputState : PlayerState
{
    [Export] private PlayerState _nextPlacingState;

    [Export] private PlayerInput _inputEvents;
    [Export] private BlockBoard _blockBoard;
    [Export] private PlayerHand _playerHand;

    public override void OnEnter()
    {
        _inputEvents.LeftPressed += MoveLeft;
        _inputEvents.RightPressed += MoveRight;
        _inputEvents.ClockwisePressed += RotateClockwise;
        _inputEvents.AnticlockwisePressed += RotateAnticlockwise;
        _inputEvents.ConfirmPressed += PlaceCurrentPolyomino;
    }

    public override void OnExit()
    {
        _playerHand.CurPolyomino.Tween?.Kill();
        _inputEvents.LeftPressed -= MoveLeft;
        _inputEvents.RightPressed -= MoveRight;
        _inputEvents.ClockwisePressed -= RotateClockwise;
        _inputEvents.AnticlockwisePressed -= RotateAnticlockwise;
        _inputEvents.ConfirmPressed -= PlaceCurrentPolyomino;
    }

    public void MoveLeft()
    {
        _playerHand.LeftIndex--;
        _playerHand.WallKick(_blockBoard);
        _blockBoard.UpdatePreviewPolyomino(_playerHand.CurPolyomino, _playerHand.LeftIndex);
    }

    public void MoveRight()
    {
        _playerHand.LeftIndex++;
        _playerHand.WallKick(_blockBoard);
        _blockBoard.UpdatePreviewPolyomino(_playerHand.CurPolyomino, _playerHand.LeftIndex);
    }

    public async void RotateClockwise()
    {
        var task = _playerHand.CurPolyomino.Rotate(clockwise: true);
        _playerHand.WallKick(_blockBoard);
        _blockBoard.UpdatePreviewPolyomino(_playerHand.CurPolyomino, _playerHand.LeftIndex);
        await task;
    }

    public async void RotateAnticlockwise()
    {
        var task = _playerHand.CurPolyomino.Rotate(clockwise: false);
        _playerHand.WallKick(_blockBoard);
        _blockBoard.UpdatePreviewPolyomino(_playerHand.CurPolyomino, _playerHand.LeftIndex);
        await task;
    }

    public void PlaceCurrentPolyomino()
    {
        EmitSignal(SignalName.Transitioned, _nextPlacingState);
    }
}
