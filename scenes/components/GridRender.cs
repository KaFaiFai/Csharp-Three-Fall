using Godot;
using System;
using System.Collections.Generic;

public partial class GridRender : Node2D
{
    [Export] public BlockGrid BlockGrid { get; set; }
    [Export] private float InnerBorderWidth { get; set; } = 10;
    [Export] private float OuterBorderWidth { get; set; } = 4;

    private int _topRow;

    public override void _Ready()
    {
        BlockBoard board = GetNodeOrNull<BlockBoard>("..");
        _topRow = board == null ? 0 : board.OverflowFrom;
    }

    public override void _Draw()
    {
        DrawGridLines();
        DrawInnerBorder();
        DrawOuterBorder();
    }

    private void DrawGridLines()
    {
        int numRow = BlockGrid.Blocks.GetLength(0);
        int numCol = BlockGrid.Blocks.GetLength(1);
        for (int i = _topRow + 2; i < numRow; i++)
        {
            Vector2 from = BlockGrid.GetCellPositionAt(new Vector2(i - 0.5f, -0.5f));
            Vector2 to = BlockGrid.GetCellPositionAt(new Vector2(i - 0.5f, numCol - 0.5f));
            DrawLine(from, to, Colors.Gray, 2.0f);
        }
        for (int j = 1; j < numCol; j++)
        {
            Vector2 from = BlockGrid.GetCellPositionAt(new Vector2(_topRow + 0.5f, j - 0.5f));
            Vector2 to = BlockGrid.GetCellPositionAt(new Vector2(numRow - 0.5f, j - 0.5f));
            DrawLine(from, to, Colors.Gray, 2.0f);
        }
    }

    private void DrawInnerBorder()
    {
        int numRow = BlockGrid.Blocks.GetLength(0);
        int numCol = BlockGrid.Blocks.GetLength(1);
        Vector2 start = BlockGrid.GetCellPositionAt(new Vector2(_topRow + 0.5f, -0.5f))
            - Vector2.One * InnerBorderWidth / 2;
        Vector2 end = BlockGrid.GetCellPositionAt(new Vector2(numRow - 0.5f, numCol - 0.5f))
            + Vector2.One * InnerBorderWidth / 2;
        Vector2 size = end - start;
        Rect2 rect = new Rect2(start, size);
        DrawRect(rect, Colors.Blue, false, InnerBorderWidth);
    }

    private void DrawOuterBorder()
    {
        int numRow = BlockGrid.Blocks.GetLength(0);
        int numCol = BlockGrid.Blocks.GetLength(1);
        Vector2 start = BlockGrid.GetCellPositionAt(new Vector2(_topRow + 0.5f, -0.5f))
            - Vector2.One * (InnerBorderWidth + OuterBorderWidth / 2);
        Vector2 end = BlockGrid.GetCellPositionAt(new Vector2(numRow - 0.5f, numCol - 0.5f))
            + Vector2.One * (InnerBorderWidth + OuterBorderWidth / 2);
        Vector2 size = end - start;
        Rect2 rect = new Rect2(start, size);
        DrawRect(rect, Colors.DarkGray, false, OuterBorderWidth);
    }
}
