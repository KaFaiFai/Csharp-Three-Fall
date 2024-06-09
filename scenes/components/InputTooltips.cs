using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class InputTooltips : VBoxContainer
{
    [Export] public TextureRect Left { get; private set; }
    [Export] public TextureRect Right { get; private set; }
    [Export] public TextureRect Clockwise { get; private set; }
    [Export] public TextureRect Anticlockwise { get; private set; }
    [Export] public TextureRect ConfirmButton { get; private set; }

    static private String InputImagesFolder { get; } = "res://assets/images/inputs/";

    public void SetLabels(List<int> playerIndices)
    {
        if (playerIndices == null || playerIndices.Count == 0) { return; }
        int playerIndex = playerIndices.Min();
        UpdateImageWithAction(Left, $"left-player{playerIndex}");
        UpdateImageWithAction(Right, $"right-player{playerIndex}");
        UpdateImageWithAction(Clockwise, $"clockwise-player{playerIndex}");
        UpdateImageWithAction(Anticlockwise, $"anticlockwise-player{playerIndex}");
        UpdateImageWithAction(ConfirmButton, $"confirm-player{playerIndex}");
    }

    private void UpdateImageWithAction(TextureRect textureRect, String action)
    {
        try
        {
            Array<InputEvent> events = InputMap.ActionGetEvents(action);
            InputEventKey keyEvent = (InputEventKey)events.First();
            string key = OS.GetKeycodeString(keyEvent.PhysicalKeycode);
            textureRect.Texture = GD.Load<Texture2D>($"{InputImagesFolder}keyboard_{key.ToLower()}.png");
        }
        catch (Exception)
        {
            throw;
        }
    }
}
