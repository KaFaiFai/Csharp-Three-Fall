using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;

public partial class Polyomino : Node2D
{
    [Export]
    public PackedScene BlockScene;

    static private readonly float _cellSize = 50;
    public Block[,] Blocks { get; set; }

    // For rotation animations
    private Tween _tween;

    public override void _Ready()
    {
        BlockType?[,] blockTypes = new BlockType?[,] { { BlockType.a, BlockType.b }, { null, BlockType.c } };
        UpdateBlocksFromTypes(blockTypes);
    }

    public void RelocateBlocks()
    {
        Vector2I rowCol = new Vector2I(Blocks.GetLength(0), Blocks.GetLength(1));
        for (var i = 0; i < rowCol.X; i++)
        {
            for (var j = 0; j < rowCol.Y; j++)
            {
                Block block = Blocks[i, j];
                if (block != null)
                {
                    block.Position = Util.GetCellPositionAt(rowCol, _cellSize, new Vector2I(i, j));
                }
            }
        }
    }

    public void UpdateBlocksFromTypes(BlockType?[,] blockTypes)
    {
        // Remove previous blocks
        foreach (var child in GetChildren())
        {
            if (child is Block)
            {
                child.QueueFree();
            }
        }

        // Instantiate new blocks
        Vector2I gridRowCol = new Vector2I(blockTypes.GetLength(0), blockTypes.GetLength(1));
        Blocks = new Block[gridRowCol.X, gridRowCol.Y];
        for (var i = 0; i <= blockTypes.GetLength(0); i++)
        {
            for (var j = 0; j < blockTypes.GetLength(1); j++)
            {
                BlockType? blockType = blockTypes[i, j];
                if (blockType != null)
                {
                    Block block = BlockScene.Instantiate<Block>();
                    block.Type = (BlockType)blockType;
                    block.Position = Util.GetCellPositionAt(gridRowCol, _cellSize, new Vector2I(i, j));
                    AddChild(block);
                    Blocks[i, j] = block;
                }
            }
        }
    }

    public void RotateClockwise() => Rotate(clockwise: true);

    public void RotateAnticlockwise() => Rotate(clockwise: false);

    private async void Rotate(bool clockwise)
    {
        Vector2I rowCol = new Vector2I(Blocks.GetLength(0), Blocks.GetLength(1));
        List<(Vector2I, Vector2I)> gridRotations = Util.RotateGridCells(rowCol, clockwise: clockwise);
        Vector2I rotatedRowCol = new Vector2I(rowCol.X, rowCol.Y);

        Block[,] newBlocks = new Block[rotatedRowCol.X, rotatedRowCol.Y];
        _tween?.Kill();
        _tween = CreateTween().SetParallel();
        foreach (var (from, to) in gridRotations)
        {
            Block block = Blocks[from.X, from.Y];
            if (block != null)
            {
                newBlocks[to.X, to.Y] = block;
                Vector2 targetPosition = Util.GetCellPositionAt(rotatedRowCol, _cellSize, to);
                _tween.TweenMethod(TweenRotation(block, targetPosition, clockwise), 0f, 1f, 1.0f);
            }
        }
        Blocks = newBlocks;
        await ToSignal(_tween, Tween.SignalName.Finished);
        RelocateBlocks();
    }

    private Callable TweenRotation(Node2D scene, Vector2 targetPosition, bool clockwise)
    {
        Vector2 originalPosition = scene.Position;
        void Rotate(float t)
        {
            scene.Position = Util.CurvedInterpolate(originalPosition, targetPosition, Vector2.Zero, clockwise, t);
        }
        return Callable.From<float>(Rotate);
    }
}
