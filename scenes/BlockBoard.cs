using Godot;
using System;
using System.Collections.Generic;

public partial class BlockBoard : Node2D
{
    [Signal]
    public delegate void BoardResolvedEventHandler();

    [Export]
    public PackedScene BlockScene;

    public BlockGrid BlockGrid { get; set; }
    public List<Block> PreviewBlocks { get; set; } = new List<Block>();

    public override void _Ready()
    {
        BlockGrid = GetNode<BlockGrid>("BlockGrid");
    }

    public void ClearPreview()
    {
        foreach (var block in PreviewBlocks)
        {
            block.QueueFree();
        }
        PreviewBlocks = new List<Block>();
    }

    public void UpdatePreviewPolyomino(Polyomino polyomino, int leftIndex)
    {
        ClearPreview();

        List<(Vector2I, BlockType)> willDropTo =
            Mechanics.WillDropTo(BlockGrid.ToBlockTypes(), polyomino.BlockGrid.ToBlockTypes(), leftIndex);
        foreach (var (rowCol, type) in willDropTo)
        {
            Block newBlock = BlockScene.Instantiate<Block>();
            newBlock.Type = type;
            newBlock.Position = BlockGrid.GetCellPositionAt(rowCol);
            newBlock.Modulate = new Color(newBlock.Modulate, 0.5f);
            PreviewBlocks.Add(newBlock);
            AddChild(newBlock);
        }

    }

    public async void ResolveBoardForNewPolyomino(Polyomino polyomino, int leftIndex)
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
        SceneTreeTimer timer = GetTree().CreateTimer(1);
        await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
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
