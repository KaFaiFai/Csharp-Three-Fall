using Godot;
using System;

public partial class InputEvents : Node
{
    [Signal]
    public delegate void LeftPressedEventHandler();
    [Signal]
    public delegate void RightPressedEventHandler();
    [Signal]
    public delegate void ClockwisePressedEventHandler();
    [Signal]
    public delegate void AnticlockwisePressedEventHandler();
    [Signal]
    public delegate void ConfirmPressedEventHandler();

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("left"))
        {
            EmitSignal(SignalName.LeftPressed);
        }
        if (Input.IsActionJustPressed("right"))
        {
            EmitSignal(SignalName.RightPressed);
        }
        if (Input.IsActionJustPressed("clockwise"))
        {
            EmitSignal(SignalName.ClockwisePressed);
        }
        if (Input.IsActionJustPressed("anticlockwise"))
        {
            EmitSignal(SignalName.AnticlockwisePressed);
        }
        if (Input.IsActionJustPressed("confirm"))
        {
            EmitSignal(SignalName.ConfirmPressed);
        }
    }
}
