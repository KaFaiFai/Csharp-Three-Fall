using Godot;
using System;
using System.Collections.Generic;

public partial class BlockGrid : Node2D
{
    [Export]
    public PackedScene BlockScene;

    public float CellSize { get; set; } = 50;
    public Block[,] Blocks { get; set; }

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

    /// <summary>
    /// Convienient method using this data
    /// </summary>
    public Vector2 GetCellPositionAt(Vector2I cellRowCol)
    {
        Vector2I gridRowCol = new Vector2I(Blocks.GetLength(0), Blocks.GetLength(1));
        return GetCellPositionAt(gridRowCol, CellSize, cellRowCol);
    }

    /// <summary>
    /// Return the from and to of each cell position
    /// </summary>
    public List<(Vector2I, Vector2I)> RotateCells(bool clockwise)
    {
        Vector2I rowCol = new Vector2I(Blocks.GetLength(0), Blocks.GetLength(1));
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
                    block.Position = GetCellPositionAt(new Vector2I(i, j));
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
        for (var i = 0; i < blockTypes.GetLength(0); i++)
        {
            for (var j = 0; j < blockTypes.GetLength(1); j++)
            {
                BlockType? blockType = blockTypes[i, j];
                if (blockType != null)
                {
                    Block block = BlockScene.Instantiate<Block>();
                    block.Type = (BlockType)blockType;
                    block.Position = GetCellPositionAt(gridRowCol, CellSize, new Vector2I(i, j));
                    AddChild(block);
                    Blocks[i, j] = block;
                }
            }
        }
    }

    public BlockType?[,] ToBlockTypes()
    {
        BlockType?[,] blockTypes = new BlockType?[Blocks.GetLength(0), Blocks.GetLength(1)];
        for (var i = 0; i < Blocks.GetLength(0); i++)
        {
            for (var j = 0; j < Blocks.GetLength(1); j++)
            {
                blockTypes[i, j] = Blocks[i, j]?.Type;
            }
        }
        return blockTypes;
    }
}
