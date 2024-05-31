using Godot;
using System;

public partial class InputEvents : Node
{
    [Signal]
    public delegate void MovedLeftEventHandler();
    [Signal]
    public delegate void MovedRightEventHandler();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("move_left"))
        {
            EmitSignal(SignalName.MovedLeft);
        }
        if (Input.IsActionJustPressed("move_right"))
        {
            EmitSignal(SignalName.MovedRight);
        }
    }
}
