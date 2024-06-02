using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;

public partial class Polyomino : Node2D
{
    private BlockGrid _blockGrid;

    // For rotation animations
    private Tween _tween;

    public override void _Ready()
    {
        _blockGrid = GetNode<BlockGrid>("BlockGrid");
        BlockType?[,] blockTypes = new BlockType?[,] { { BlockType.a, BlockType.b }, { null, BlockType.c } };
        _blockGrid.UpdateBlocksFromTypes(blockTypes);
    }

    public void RotateClockwise() => Rotate(clockwise: true);

    public void RotateAnticlockwise() => Rotate(clockwise: false);

    private async void Rotate(bool clockwise)
    {
        List<(Vector2I, Vector2I)> gridRotations = _blockGrid.RotateCells(clockwise: clockwise);
        Vector2I rotatedRowCol = new Vector2I(_blockGrid.Blocks.GetLength(1), _blockGrid.Blocks.GetLength(0));

        Block[,] newBlocks = new Block[rotatedRowCol.X, rotatedRowCol.Y];
        _tween?.Kill();
        _tween = CreateTween().SetParallel();
        foreach (var (from, to) in gridRotations)
        {
            Block block = _blockGrid.Blocks[from.X, from.Y];
            if (block != null)
            {
                newBlocks[to.X, to.Y] = block;
                Vector2 targetPosition = BlockGrid.GetCellPositionAt(rotatedRowCol, _blockGrid.CellSize, to);
                _tween.TweenMethod(TweenRotation(block, targetPosition, clockwise), 0f, 1f, 1.0f);
            }
        }
        _blockGrid.Blocks = newBlocks;
        await ToSignal(_tween, Tween.SignalName.Finished);
        _blockGrid.RelocateBlocks();
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
