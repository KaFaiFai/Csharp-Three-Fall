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

    //public void _on_timer_timeout()
    //{
    //    GD.Print("Hello");
    //}
}
