using Godot;
using System;

public partial class Block : Node
{
    public BlockType Type { get; set; } = BlockType.d;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ColorRect colorRect = GetNode<ColorRect>("ColorRect");
        colorRect.Color = Type switch
        {
            BlockType.a => Colors.Blue,
            BlockType.b => Colors.Violet,
            BlockType.c => Colors.Yellow,
            BlockType.d => Colors.Green,
            _ => Colors.Black,
        };
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    private async void _on_input_events_moved_left()
    {
        GD.Print("Moved Left");
        Tween tween = CreateTween();
        tween.TweenProperty(this, "position", new Vector2(300, 300), 1f);
        await ToSignal(tween, "finished");
        GD.Print("Moved Left Finished");
        tween = CreateTween();
        tween.TweenProperty(this, "position", new Vector2(200, 300), 1f);
    }
    private async void _on_input_events_moved_right()
    {
        GD.Print("Moved Right");
        Tween tween = CreateTween();
        tween.TweenProperty(this, "position", new Vector2(700, 300), 1f);
        await ToSignal(tween, "finished");
        GD.Print("Moved Right Finished");
        tween = CreateTween();
        tween.TweenProperty(this, "position", new Vector2(800, 300), 1f);
    }
}
