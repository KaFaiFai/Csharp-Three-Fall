using Godot;
using System;
using System.Threading.Tasks;

public partial class GameScreen : Node2D
{
    public RandomNumberGenerator Rng { get; set; }
    public int LeftIndex { get; set; } = 0;

    public Polyomino CurPolyomino { get; private set; }
    public Polyomino NextPolyomino { get; private set; }
    public BlockBoard BlockBoard { get; private set; }

    public int OverflowFrom { get; set; } = 3;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Rng = new RandomNumberGenerator() { Seed = 0 };

        CurPolyomino = GetNode<Polyomino>("CurrentPolyomino");
        NextPolyomino = GetNode<Polyomino>("NextPolyomino");
        BlockBoard = GetNode<BlockBoard>("BlockBoard");

        AdvancePolyomino();
        AdvancePolyomino();
        BlockBoard.BlockGrid.UpdateBlocksFromTypes(new BlockType?[10, 6]); // Some top rows are for overflow blocks
        WallKick();
        BlockBoard.UpdatePreviewPolyomino(CurPolyomino, LeftIndex);
    }

    public void AdvancePolyomino()
    {
        CurPolyomino.BlockGrid.UpdateBlocksFromTypes(NextPolyomino.BlockGrid.ToBlockTypes());

        // TODO: Randomize shapes
        bool[,] shape = new bool[,] { { true, false, false }, { true, true, true } };
        BlockType?[,] newBlockTypes = new BlockType?[shape.GetLength(0), shape.GetLength(1)];
        BlockType[] allBlockTypes = Enum.GetValues<BlockType>();
        for (int i = 0; i < shape.GetLength(0); i++)
        {
            for (int j = 0; j < shape.GetLength(1); j++)
            {
                if (shape[i, j])
                {
                    newBlockTypes[i, j] = allBlockTypes[Rng.RandiRange(0, allBlockTypes.Length - 1)];
                }
            }
        }
        NextPolyomino.BlockGrid.UpdateBlocksFromTypes(newBlockTypes);
    }

    public void WallKick()
    {
        int baordWidth = BlockBoard.BlockGrid.Blocks.GetLength(1);
        int blocksWidth = CurPolyomino.BlockGrid.Blocks.GetLength(1);
        LeftIndex = Mechanics.WallKick(baordWidth, blocksWidth, LeftIndex);
    }

    public bool HasOverflowBlocks()
    {
        for (var i = 0; i <= OverflowFrom; i++)
        {
            for (var j = 0; j < BlockBoard.BlockGrid.Blocks.GetLength(1); j++)
            {
                if (BlockBoard.BlockGrid.Blocks[i, j] != null) return true;
            }
        }
        return false;
    }
}
