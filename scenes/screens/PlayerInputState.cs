using Godot;
using System;

public partial class PlayerInputState : PlayerState
{
    [Signal]
    public delegate void EnteredNewTurnEventHandler();

    [Export]
    private InputEvents _inputEvents;

    public override void OnEnter()
    {
        EmitSignal(SignalName.EnteredNewTurn);
        _inputEvents.LeftPressed += MoveLeft;
        _inputEvents.RightPressed += MoveRight;
        _inputEvents.ClockwisePressed += RotateClockwise;
        _inputEvents.AnticlockwisePressed += RotateAnticlockwise;
        _inputEvents.ConfirmPressed += PlaceCurrentPolyomino;
    }

    public override void OnExit()
    {
        CurPolyomino.Tween?.Kill();
        _inputEvents.LeftPressed -= MoveLeft;
        _inputEvents.RightPressed -= MoveRight;
        _inputEvents.ClockwisePressed -= RotateClockwise;
        _inputEvents.AnticlockwisePressed -= RotateAnticlockwise;
        _inputEvents.ConfirmPressed -= PlaceCurrentPolyomino;
    }

    public void MoveLeft()
    {
        GameSession.LeftIndex--;
        GameSession.WallKick();
        BlockBoard.UpdatePreviewPolyomino(CurPolyomino, GameSession.LeftIndex);
    }

    public void MoveRight()
    {
        GameSession.LeftIndex++;
        GameSession.WallKick();
        BlockBoard.UpdatePreviewPolyomino(CurPolyomino, GameSession.LeftIndex);
    }

    public async void RotateClockwise()
    {
        var task = CurPolyomino.Rotate(clockwise: true);
        GameSession.WallKick();
        BlockBoard.UpdatePreviewPolyomino(CurPolyomino, GameSession.LeftIndex);
        await task;
    }

    public async void RotateAnticlockwise()
    {
        var task = CurPolyomino.Rotate(clockwise: false);
        GameSession.WallKick();
        BlockBoard.UpdatePreviewPolyomino(CurPolyomino, GameSession.LeftIndex);
        await task;
    }


    public void PlaceCurrentPolyomino()
    {
        EmitSignal(SignalName.Transitioned, "PlayerPlacingState");
    }
}
