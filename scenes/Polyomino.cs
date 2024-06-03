using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public partial class Polyomino : Node2D
{
    public BlockGrid BlockGrid { get; set; }

    // For rotation animations
    private Tween _tween;

    public override void _Ready()
    {
        BlockGrid = GetNode<BlockGrid>("BlockGrid");
        BlockType?[,] blockTypes = new BlockType?[,] { { BlockType.a, BlockType.b }, { null, BlockType.c } };
        BlockGrid.UpdateBlocksFromTypes(blockTypes);
    }

    public async Task Rotate(bool clockwise)
    {
        List<(Vector2I, Vector2I)> gridRotations = BlockGrid.RotateCells(clockwise: clockwise);
        Vector2I rotatedRowCol = new Vector2I(BlockGrid.Blocks.GetLength(1), BlockGrid.Blocks.GetLength(0));

        Block[,] newBlocks = new Block[rotatedRowCol.X, rotatedRowCol.Y];
        _tween?.Kill();
        _tween = CreateTween().SetParallel();
        foreach (var (from, to) in gridRotations)
        {
            Block block = BlockGrid.Blocks[from.X, from.Y];
            if (block != null)
            {
                newBlocks[to.X, to.Y] = block;
                Vector2 targetPosition = BlockGrid.GetCellPositionAt(rotatedRowCol, BlockGrid.CellSize, to);
                _tween.TweenMethod(TweenRotation(block, targetPosition, clockwise), 0f, 1f, 1.0f);
            }
        }
        BlockGrid.Blocks = newBlocks;
        await ToSignal(_tween, Tween.SignalName.Finished);
        BlockGrid.RelocateBlocks();
        return;
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
