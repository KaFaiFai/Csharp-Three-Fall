using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PlayerInput : Node
{
    [Signal] public delegate void LeftPressedEventHandler();
    [Signal] public delegate void RightPressedEventHandler();
    [Signal] public delegate void ClockwisePressedEventHandler();
    [Signal] public delegate void AnticlockwisePressedEventHandler();
    [Signal] public delegate void ConfirmPressedEventHandler();

    // Expect "1", "2", "1,2", etc
    [Export] public String Players;

    private List<int> _playerIndices;

    public override void _Ready()
    {
        _playerIndices = Players.Split(',').Select(c => c.ToInt()).ToList();
        GetNode<InputTooltips>("InputTooltips").SetLabels(_playerIndices);
    }

    public override void _Process(double delta)
    {
        if (AnyActionPressed(i => $"left-player{i}"))
        {
            EmitSignal(SignalName.LeftPressed);
        }
        if (AnyActionPressed(i => $"right-player{i}"))
        {
            EmitSignal(SignalName.RightPressed);
        }
        if (AnyActionPressed(i => $"clockwise-player{i}"))
        {
            EmitSignal(SignalName.ClockwisePressed);
        }
        if (AnyActionPressed(i => $"anticlockwise-player{i}"))
        {
            EmitSignal(SignalName.AnticlockwisePressed);
        }
        if (AnyActionPressed(i => $"confirm-player{i}"))
        {
            EmitSignal(SignalName.ConfirmPressed);
        }
    }

    private bool AnyActionPressed(Func<int, String> actionName)
    {
        return _playerIndices.Any(i => Input.IsActionJustPressed(actionName(i)));
    }
}
