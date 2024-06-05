using Godot;
using System;

public partial class GameScreenInputState : GameScreenState
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
        GameScreen.LeftIndex--;
        GameScreen.WallKick();
        BlockBoard.UpdatePreviewPolyomino(CurPolyomino, GameScreen.LeftIndex);
    }

    public void MoveRight()
    {
        GameScreen.LeftIndex++;
        GameScreen.WallKick();
        BlockBoard.UpdatePreviewPolyomino(CurPolyomino, GameScreen.LeftIndex);
    }

    public async void RotateClockwise()
    {
        var task = CurPolyomino.Rotate(clockwise: true);
        GameScreen.WallKick();
        BlockBoard.UpdatePreviewPolyomino(CurPolyomino, GameScreen.LeftIndex);
        await task;
    }

    public async void RotateAnticlockwise()
    {
        var task = CurPolyomino.Rotate(clockwise: false);
        GameScreen.WallKick();
        BlockBoard.UpdatePreviewPolyomino(CurPolyomino, GameScreen.LeftIndex);
        await task;
    }


    public void PlaceCurrentPolyomino()
    {
        EmitSignal(SignalName.Transitioned, "GameScreenPlacingState");
    }
}
