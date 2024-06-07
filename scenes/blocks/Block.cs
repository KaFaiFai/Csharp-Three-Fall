using Godot;
using System;

public partial class Block : Node2D
{
    public BlockType Type { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        int frame = Type switch
        {
            BlockType.a => 0,
            BlockType.b => 1,
            BlockType.c => 2,
            BlockType.d => 3,
            _ => 4,
        };
        var frameSize = sprite.SpriteFrames.GetFrameTexture("tiles", frame).GetSize();

        // fix the size to be Constants.BlockSize
        sprite.Scale = Vector2.One * Constants.BlockSize / Math.Max(frameSize.X, frameSize.Y);
        sprite.Frame = frame;
    }
}
