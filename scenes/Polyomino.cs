using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

public partial class Polyomino : Node2D
{
    [Export]
    public PackedScene BlockScene;

    GridLayout GridLayout { get; set; }
    public int[,] Shape { get; set; }
    public List<BlockType> BlockTypes { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Shape = new int[,] { { 0, 1 }, { -1, 2 } };
        BlockTypes = new List<BlockType> { BlockType.a, BlockType.b, BlockType.c };

        GridLayout = GetNode<GridLayout>("GridLayout");
        GridLayout.CellSize = 50;
        GridLayout.RowCol = new Vector2I(Shape.GetLength(0), Shape.GetLength(1));
        RedrawBlocks();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void RedrawBlocks()
    {
        foreach (var child in GetChildren())
        {
            if (child is Block)
            {
                child.QueueFree();
            }
        }
        GridLayout.RowCol = new Vector2I(Shape.GetLength(0), Shape.GetLength(1));
        for (var i = 0; i < Shape.GetLength(0); i++)
        {
            for (var j = 0; j < Shape.GetLength(1); j++)
            {
                int typeIndex = Shape[i, j];
                if (typeIndex != -1)
                {
                    Block block = BlockScene.Instantiate<Block>();
                    block.Type = BlockTypes[typeIndex];
                    block.Position = GridLayout.At(new Vector2I(i, j));
                    AddChild(block);
                }
            }
        }
    }

    public void RotateClockwise() => Rotate(clockwise: true);

    public void RotateAnticlockwise() => Rotate(clockwise: false);

    private void Rotate(bool clockwise)
    {
        if (clockwise)
        {
            Shape = RotateMatrixClockwise(Shape);
        }
        else
        {
            // Rotate 3 times clockwise for anti-clockwise
            Shape = RotateMatrixClockwise(RotateMatrixClockwise(RotateMatrixClockwise(Shape)));
        }
        //RedrawBlocks();

        Tween tween = CreateTween().SetParallel();
        foreach (var child in GetChildren())
        {
            if (child is Block)
            {
                tween.TweenMethod(TweenRotation((Block)child), 0, 90, 1.0f);
            }
        }
    }

    private Callable TweenRotation(Node2D scene)
    {
        Vector2 start = scene.Position;
        void Rotate(int degrees)
        {
            scene.Position = RotatePoint(start, Vector2.Zero, degrees);
        }
        return Callable.From<int>(Rotate);
    }

    static private T[,] RotateMatrixClockwise<T>(T[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        T[,] rotatedMatrix = new T[cols, rows];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                rotatedMatrix[j, rows - 1 - i] = matrix[i, j];
            }
        }
        return rotatedMatrix;
    }
    static private Vector2 RotatePoint(Vector2 point, Vector2 center, float degrees)
    {
        var radians = Mathf.DegToRad(degrees);
        var cosAngle = Mathf.Cos(radians);
        var sinAngle = Mathf.Sin(radians);
        var translatedPoint = point - center;
        var rotatedPoint = new Vector2(
            translatedPoint.X * cosAngle - translatedPoint.Y * sinAngle,
            translatedPoint.X * sinAngle + translatedPoint.Y * cosAngle
        );
        return rotatedPoint + center;
    }
}
