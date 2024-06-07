using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class InputTooltips : VBoxContainer
{
    [Export] public Label Left { get; private set; }
    [Export] public Label Right { get; private set; }
    [Export] public Label Clockwise { get; private set; }
    [Export] public Label Anticlockwise { get; private set; }
    [Export] public Label ConfirmButton { get; private set; }

    public void SetLabels(List<int> playerIndices)
    {
        if (playerIndices == null || playerIndices.Count == 0) { return; }
        int playerIndex = playerIndices.Min();
        UpdateLabelWithAction(Left, $"left-player{playerIndex}");
        UpdateLabelWithAction(Right, $"right-player{playerIndex}");
        UpdateLabelWithAction(Clockwise, $"clockwise-player{playerIndex}");
        UpdateLabelWithAction(Anticlockwise, $"anticlockwise-player{playerIndex}");
        UpdateLabelWithAction(ConfirmButton, $"confirm-player{playerIndex}");
    }

    private void UpdateLabelWithAction(Label label, String action)
    {
        try
        {
            Array<InputEvent> events = InputMap.ActionGetEvents(action);
            InputEventKey keyEvent = (InputEventKey)events.First();
            label.Text = OS.GetKeycodeString(keyEvent.PhysicalKeycode);
        }
        catch (Exception)
        {
            throw;
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
