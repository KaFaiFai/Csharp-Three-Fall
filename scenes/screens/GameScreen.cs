using Godot;
using System;

public partial class GameScreen : Control
{
    public String GameMode { get; set; }
    PackedScene normalScene = GD.Load<PackedScene>("res://scenes/game_modes/NormalGame.tscn");
    PackedScene cooperativeScene = GD.Load<PackedScene>("res://scenes/game_modes/CooperativeGame.tscn");
    PackedScene competitiveScene = GD.Load<PackedScene>("res://scenes/game_modes/CompetitiveGame.tscn");

    public override void _Ready()
    {
        switch (GameMode)
        {
            case "SinglePlayer":
                AddChild(normalScene.Instantiate());
                break;
            case "Cooperative":
                AddChild(cooperativeScene.Instantiate());
                break;
            case "Competitive":
                AddChild(competitiveScene.Instantiate());
                break;
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
