using Godot;
using System;

public partial class BlockBoard : Node2D
{
    private BlockGrid _blockGrid;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _blockGrid = GetNode<BlockGrid>("BlockGrid");
        BlockType?[,] blockTypes = new BlockType?[10, 6];
        blockTypes[0, 0] = BlockType.a;
        blockTypes[blockTypes.GetLength(0) - 1, 0] = BlockType.b;
        blockTypes[0, blockTypes.GetLength(1) - 1] = BlockType.c;
        blockTypes[blockTypes.GetLength(0) - 1, blockTypes.GetLength(1) - 1] = BlockType.d;
        _blockGrid.UpdateBlocksFromTypes(blockTypes);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
