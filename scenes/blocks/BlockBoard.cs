using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class BlockBoard : Node2D
{
    [Signal] public delegate void EnteredNewTurnEventHandler();
    [Signal] public delegate void EnteredPhaseEventHandler(int phase);
    [Signal] public delegate void BlocksRemovedEventHandler(int numRemoved, BlockType blockType);

    [Export] public PackedScene BlockScene;
    [Export] public PackedScene ExplosionEffect;
    [Export] public AudioStreamPlayer ComboSfxPlayer;
    [Export] public BlockGrid BlockGrid { get; set; }

    public List<Block> PreviewBlocks { get; set; } = new List<Block>();
    public List<Line2D> GridLines { get; set; } = new List<Line2D> { };

    // Some top rows are for overflow blocks
    [Export] public Vector2I InitialBoardSize { get; set; } = new Vector2I(10, 6);
    public int OverflowFrom { get; init; } = 3;
    public float BlockDropSpeed { get; set; } = 1000;

    public override void _Ready()
    {
        BlockType?[,] newEmptyBoard = new BlockType?[InitialBoardSize.X, InitialBoardSize.Y];
        BlockGrid.UpdateBlocksFromTypes(newEmptyBoard);
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

        List<(Vector2I, Vector2I, BlockType)> willDropTo =
            Mechanics.WillDropTo(BlockGrid.ToBlockTypes(), polyomino.BlockGrid.ToBlockTypes(), leftIndex);
        foreach (var (_, rowCol, type) in willDropTo)
        {
            Block newBlock = BlockScene.Instantiate<Block>();
            newBlock.Type = type;
            newBlock.Position = BlockGrid.GetCellPositionAt(rowCol);
            newBlock.Modulate = new Color(newBlock.Modulate, 0.5f);
            PreviewBlocks.Add(newBlock);
            AddChild(newBlock);
        }
    }

    public bool HasOverflowBlocks()
    {
        for (var i = 0; i <= OverflowFrom; i++)
        {
            for (var j = 0; j < BlockGrid.Blocks.GetLength(1); j++)
            {
                if (BlockGrid.Blocks[i, j] != null) return true;
            }
        }
        return false;
    }

    public async Task ResolveBoardForNewPolyomino(Polyomino polyomino, int leftIndex)
    {
        EmitSignal(SignalName.EnteredNewTurn);
        int phase = 1;
        ComboSfxPlayer.PitchScale = 1;
        bool toContinue = true;
        while (toContinue)
        {
            EmitSignal(SignalName.EnteredPhase, phase);
            if (phase == 1)
                await PlaceNewPolyomino(polyomino, leftIndex);
            else
                await DropFlyingBlocks();
            await ToSignal(GetTree().CreateTimer(0.2), Timer.SignalName.Timeout);

            toContinue = await RemoveMatch3();
            await ToSignal(GetTree().CreateTimer(0.1), Timer.SignalName.Timeout);
            phase++;
        }
    }

    private async Task PlaceNewPolyomino(Polyomino polyomino, int leftIndex)
    {
        BlockType?[,] blockTypes = BlockGrid.ToBlockTypes();
        List<(Vector2I, Vector2I, BlockType)> willDropTo =
            Mechanics.WillDropTo(blockTypes, polyomino.BlockGrid.ToBlockTypes(), leftIndex);

        // play falling animation
        Tween tween = CreateTween().SetParallel();
        foreach (var (from, to, type) in willDropTo)
        {
            Block block = BlockScene.Instantiate<Block>();
            block.Position = BlockGrid.GetCellPositionAt(from);
            block.Type = type;
            AddChild(block);
            double duration = (BlockGrid.GetCellPositionAt(from) - BlockGrid.GetCellPositionAt(to)).Length()
                / BlockDropSpeed;
            tween.TweenProperty(block, "position", BlockGrid.GetCellPositionAt(to), duration);
            tween.Finished += () => block.QueueFree();
        }
        await ToSignal(tween, Tween.SignalName.Finished);

        foreach (var (_, rowCol, type) in willDropTo)
        {
            blockTypes[rowCol.X, rowCol.Y] = type;
        }
        BlockGrid.UpdateBlocksFromTypes(blockTypes);
    }

    private async Task<bool> RemoveMatch3()
    {
        BlockType?[,] blockTypes = BlockGrid.ToBlockTypes();
        List<List<Vector2I>> match3RowCols = Mechanics.FindMatch3(blockTypes);
        if (match3RowCols.Count == 0) return false;

        Tween tween = CreateTween().SetParallel();
        for (var i = 0; i < match3RowCols.Count; i++)
        {
            double delay = i * 0.4;
            List<Vector2I> curRowCols = match3RowCols[i];
            BlockType? curType = blockTypes[curRowCols[0].X, curRowCols[0].Y];
            EmitSignal(SignalName.BlocksRemoved, curRowCols.Count, (int)curType);
            Vector2 comboCenter = curRowCols
                .Select(rowCol => BlockGrid.GetCellPositionAt(rowCol))
                .Aggregate(Vector2.Zero, (value, newPos) => value + newPos) / curRowCols.Count();
            foreach (var rowCol in curRowCols)
            {
                // play disappearing animation
                Block block = BlockGrid.Blocks[rowCol.X, rowCol.Y];
                tween.TweenProperty(block, "scale", new Vector2(0, 0), 0.2).SetDelay(delay);
                tween.TweenProperty(block, "position", comboCenter, 0.2).SetDelay(delay);

                // play explosion effect
                GpuParticles2D explosionEffect = ExplosionEffect.Instantiate<GpuParticles2D>();
                explosionEffect.Position = BlockGrid.GetCellPositionAt(rowCol);
                explosionEffect.Emitting = true;
                AddChild(explosionEffect);
                explosionEffect.Finished += () => explosionEffect.QueueFree();
            }

            // play combo sfx
            GetTree().CreateTimer(delay).Timeout += () =>
            {
                ComboSfxPlayer.Play();
                ComboSfxPlayer.PitchScale = Math.Min(ComboSfxPlayer.PitchScale + 0.1f, 1.5f);
            };

        }
        await ToSignal(tween, Tween.SignalName.Finished);

        foreach (var groups in match3RowCols)
        {
            foreach (var rowCol in groups)
            {
                blockTypes[rowCol.X, rowCol.Y] = null;
            }
        }
        BlockGrid.UpdateBlocksFromTypes(blockTypes);
        return true;
    }

    private async Task<bool> DropFlyingBlocks()
    {
        BlockType?[,] blockTypes = BlockGrid.ToBlockTypes();
        List<(Vector2I, Vector2I, BlockType)> flyingBlocks = Mechanics.FindFlyingBlocks(blockTypes);
        if (flyingBlocks.Count == 0) return false;

        // play falling animation
        Tween tween = CreateTween().SetParallel();
        foreach (var (from, to, type) in flyingBlocks)
        {
            Block block = BlockGrid.Blocks[from.X, from.Y];
            double duration = (BlockGrid.GetCellPositionAt(from) - BlockGrid.GetCellPositionAt(to)).Length()
                / BlockDropSpeed;
            tween.TweenProperty(block, "position", BlockGrid.GetCellPositionAt(to), duration);
            tween.Finished += () => block.QueueFree();
        }
        await ToSignal(tween, Tween.SignalName.Finished);

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
