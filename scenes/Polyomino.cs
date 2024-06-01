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

    static private readonly float cellSize = 50;
    public Block[,] Blocks { get; set; }
    public Vector2I BlocksSize() => new Vector2I(Blocks.GetLength(0), Blocks.GetLength(1));

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Vector2I gridRowCol = new Vector2I(2, 2);
        var block1 = BlockScene.Instantiate<Block>();
        block1.Type = BlockType.a;
        block1.Position = GetCellPositionAt(gridRowCol, cellSize, new Vector2I(0, 0));
        var block2 = BlockScene.Instantiate<Block>();
        block2.Type = BlockType.b;
        block2.Position = GetCellPositionAt(gridRowCol, cellSize, new Vector2I(0, 1));
        var block4 = BlockScene.Instantiate<Block>();
        block4.Type = BlockType.c;
        block4.Position = GetCellPositionAt(gridRowCol, cellSize, new Vector2I(1, 1));

        AddChild(block1);
        AddChild(block2);
        AddChild(block4);

        Blocks = new Block[,] { { block1, block2 }, { null, block4 } };
        RedrawBlocks();
    }

    public void RedrawBlocks()
    {
        for (var i = 0; i < Blocks.GetLength(0); i++)
        {
            for (var j = 0; j < Blocks.GetLength(1); j++)
            {
                Block block = Blocks[i, j];
                if (block != null)
                {
                    block.Position = GetCellPositionAt(BlocksSize(), cellSize, new Vector2I(i, j));
                }
            }
        }
    }

    /// <summary>
    /// Return offset from the grid center to the center of this cell
    /// </summary>
    static public Vector2 GetCellPositionAt(Vector2I gridRowCol, float cellSize, Vector2I cellRowCol)
    {
        // (row, col) is mapped to (y, x)
        Vector2 cellPos = new Vector2(cellRowCol.X, cellRowCol.Y) * cellSize + Vector2.One * cellSize / 2;
        Vector2 gridCenter = new Vector2(gridRowCol.X, gridRowCol.Y) * cellSize / 2;
        Vector2 offset = cellPos - gridCenter;
        Vector2 swapped = new Vector2(offset.Y, offset.X);
        return swapped;
    }

    public void RotateClockwise() => Rotate(clockwise: true);

    public void RotateAnticlockwise() => Rotate(clockwise: false);

    private async void Rotate(bool clockwise)
    {
        List<(Vector2I, Vector2I)> gridRotations = RotateGridCells(BlocksSize(), clockwise: clockwise);
        Vector2I rotatedRowCol = new Vector2I(Blocks.GetLength(1), Blocks.GetLength(0));

        Block[,] newBLocks = new Block[rotatedRowCol.X, rotatedRowCol.Y];
        Tween tween = CreateTween().SetParallel();
        foreach (var (from, to) in gridRotations)
        {
            Block block = Blocks[from.X, from.Y];
            if (block == null)
            {
                newBLocks[to.X, to.Y] = null;
            }
            else
            {
                newBLocks[to.X, to.Y] = block;
                Vector2 targetPosition = GetCellPositionAt(rotatedRowCol, cellSize, to);
                tween.TweenMethod(TweenRotation(block, targetPosition), 0f, 1f, 1.0f);

            }
        }
        Blocks = newBLocks;
        await ToSignal(tween, Tween.SignalName.Finished);
        RedrawBlocks();
    }

    private Callable TweenRotation(Node2D scene, Vector2 targetPosition)
    {
        Vector2 originalPosition = scene.Position;
        GD.Print(originalPosition, targetPosition);
        void Rotate(float t)
        {
            scene.Position = CurvedInterpolate(originalPosition, targetPosition, Vector2.Zero, t);
        }
        return Callable.From<float>(Rotate);
    }

    /// <summary>
    /// Return the from and to of each cell position
    /// </summary>
    static private List<(Vector2I, Vector2I)> RotateGridCells(Vector2I rowCol, bool clockwise)
    {
        List<(Vector2I, Vector2I)> results = new List<(Vector2I, Vector2I)>();
        for (int i = 0; i < rowCol.X; i++)
        {
            for (int j = 0; j < rowCol.Y; j++)
            {
                Vector2I from = new Vector2I(i, j);
                Vector2I to = clockwise ? new Vector2I(j, rowCol.X - 1 - i) : new Vector2I(rowCol.Y - 1 - j, i);
                results.Add((from, to));
            }
        }
        return results;
    }

    /// <summary>
    /// Linear interpolate the angle and distance from the center. Expect 0 <= t <= 1
    /// </summary>
    static private Vector2 CurvedInterpolate(Vector2 start, Vector2 end, Vector2 center, float t)
    {
        Vector2 startToCenter = start - center;
        Vector2 endToCenter = end - center;

        float startLength = startToCenter.Length();
        float endLength = endToCenter.Length();
        float interpolatedLength = startLength * (1 - t) + endLength * t;

        // find angle between start and end
        float dotProduct = startToCenter.Dot(endToCenter);
        float magnitudeProduct = startToCenter.Length() * endToCenter.Length();
        float angle = Mathf.Acos(dotProduct / magnitudeProduct);
        float interpolatedAngle = angle * t;

        // find rotated vector of start by some angles
        float cosTheta = Mathf.Cos(interpolatedAngle);
        float sinTheta = Mathf.Sin(interpolatedAngle);
        float rotatedX = start.X * cosTheta - start.Y * sinTheta;
        float rotatedY = start.X * sinTheta + start.Y * cosTheta;
        Vector2 rotatedVector = new Vector2(rotatedX, rotatedY);

        Vector2 scaledRotatedVector = rotatedVector / rotatedVector.Length() * interpolatedLength;

        return scaledRotatedVector + center;
    }
}
