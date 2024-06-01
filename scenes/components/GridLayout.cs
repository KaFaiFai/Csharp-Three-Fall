using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class GridLayout : Node2D
{
    public float CellSize { get; set; } = 0;
    public Vector2I RowCol { get; set; } = Vector2I.Zero;

    /// <summary>
    /// Return offset from the grid center to the center of this cell
    /// </summary>
    public Vector2 At(Vector2I rowCol)
    {
        // (row, col) is mapped to (y, x)
        Vector2 cellPos = new Vector2(rowCol.X, rowCol.Y) * CellSize + Vector2.One * CellSize / 2;
        Vector2 offset = cellPos - Center();
        Vector2 swapped = new Vector2(offset.Y, offset.X);
        return swapped;
    }

    public Vector2 Center()
    {
        return new Vector2(RowCol.X, RowCol.Y) * CellSize / 2;
    }
}
