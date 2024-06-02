using Godot;
using System;

public partial class BlockBoard : Node2D
{
    [Export]
    public PackedScene BlockScene;

    static private readonly float _cellSize = 50;
    public Block[,] Blocks { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        BlockType?[,] blockTypes = new BlockType?[10, 6];
        blockTypes[0, 0] = BlockType.a;
        blockTypes[blockTypes.GetLength(0) - 1, 0] = BlockType.b;
        blockTypes[0, blockTypes.GetLength(1) - 1] = BlockType.c;
        blockTypes[blockTypes.GetLength(0) - 1, blockTypes.GetLength(1) - 1] = BlockType.d;
        UpdateBlocksFromTypes(blockTypes);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
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
                    block.Position = Util.GetCellPositionAt(rowCol, _cellSize, new Vector2I(i, j));
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
                    block.Position = Util.GetCellPositionAt(gridRowCol, _cellSize, new Vector2I(i, j));
                    AddChild(block);
                    Blocks[i, j] = block;
                }
            }
        }
    }
}
