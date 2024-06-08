using Godot;
using System;
using System.Collections.Generic;

public partial class GridRender : Node2D
{
    [Export] public BlockGrid BlockGrid { get; set; }

    public override void _Ready()
    {
        GD.Print("Ready");
    }

    public override void _Draw()
    {
        int numRow = BlockGrid.Blocks.GetLength(0);
        int numCol = BlockGrid.Blocks.GetLength(1);

        // internal gridlines
        for (int i = 1; i < numRow; i++)
        {
            Vector2 from = BlockGrid.GetCellPositionAt(new Vector2(i - 0.5f, -0.5f));
            Vector2 to = BlockGrid.GetCellPositionAt(new Vector2(i - 0.5f, numCol - 0.5f));
            DrawLine(from, to, Colors.Gray, 2.0f);
        }
        for (int j = 1; j < numCol; j++)
        {
            Vector2 from = BlockGrid.GetCellPositionAt(new Vector2(-0.5f, j - 0.5f));
            Vector2 to = BlockGrid.GetCellPositionAt(new Vector2(numRow - 0.5f, j - 0.5f));
            DrawLine(from, to, Colors.Gray, 2.0f);
        }

        // inner border
        float innerBorderWidth = 5;
        Vector2 start = BlockGrid.GetCellPositionAt(new Vector2(-0.5f, -0.5f))
            - Vector2.One * innerBorderWidth / 2;
        Vector2 end = BlockGrid.GetCellPositionAt(new Vector2(numRow - 0.5f, numCol - 0.5f))
            + Vector2.One * innerBorderWidth / 2;
        Vector2 size = end - start;
        Rect2 rect = new Rect2(start, size);
        DrawRect(rect, Colors.Blue, false, innerBorderWidth);
    }
}
