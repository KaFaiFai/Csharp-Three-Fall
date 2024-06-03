using Godot;
using System;
using System.Collections.Generic;

public partial class BlockBoard : Node2D
{
    [Signal]
    public delegate void BoardResolvedEventHandler();

    public BlockGrid BlockGrid;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        BlockGrid = GetNode<BlockGrid>("BlockGrid");
        BlockType?[,] blockTypes = new BlockType?[10, 6];
        blockTypes[0, 0] = BlockType.a;
        blockTypes[blockTypes.GetLength(0) - 1, 0] = BlockType.b;
        blockTypes[0, blockTypes.GetLength(1) - 1] = BlockType.c;
        blockTypes[blockTypes.GetLength(0) - 1, blockTypes.GetLength(1) - 1] = BlockType.d;
        BlockGrid.UpdateBlocksFromTypes(blockTypes);
    }

    public void ResolveBoardForNewPolyomino(Polyomino polyomino, int leftIndex)
    {
        PlaceNewPolyomino(polyomino, leftIndex);
        bool toContinue = true;
        int phase = 0;
        while (toContinue)
        {
            RemoveMatch3();
            toContinue = DropFlyingBlocks();
            phase++;
        }
        EmitSignal(SignalName.BoardResolved);
    }

    private void PlaceNewPolyomino(Polyomino polyomino, int leftIndex)
    {
        BlockType?[,] blockTypes = BlockGrid.ToBlockTypes();
        List<(Vector2I, BlockType)> willDropTo =
            Mechanics.WillDropTo(blockTypes, polyomino.BlockGrid.ToBlockTypes(), leftIndex);
        foreach (var (rowCol, type) in willDropTo)
        {
            blockTypes[rowCol.X, rowCol.Y] = type;
        }
        BlockGrid.UpdateBlocksFromTypes(blockTypes);
    }

    private bool RemoveMatch3()
    {
        BlockType?[,] blockTypes = BlockGrid.ToBlockTypes();
        List<(Vector2I, BlockType)> match3Blocks = Mechanics.FindMatch3(blockTypes);
        if (match3Blocks.Count == 0) return false;

        foreach (var (rowCol, _) in match3Blocks)
        {
            blockTypes[rowCol.X, rowCol.Y] = null;
        }
        BlockGrid.UpdateBlocksFromTypes(blockTypes);
        return true;
    }

    private bool DropFlyingBlocks()
    {
        BlockType?[,] blockTypes = BlockGrid.ToBlockTypes();
        List<(Vector2I, Vector2I, BlockType)> flyingBlocks = Mechanics.FindFlyingBlocks(blockTypes);
        if (flyingBlocks.Count == 0) return false;

        foreach (var (start, _, _) in flyingBlocks)
        {
            blockTypes[start.X, start.Y] = null;
        }
        foreach (var (_, end, type) in flyingBlocks)
        {
            blockTypes[end.X, end.Y] = type;
        }
        BlockGrid.UpdateBlocksFromTypes(blockTypes);
        return true;
    }
}
