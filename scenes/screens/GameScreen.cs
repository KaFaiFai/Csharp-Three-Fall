using Godot;
using System;
using System.Threading.Tasks;

public partial class GameScreen : Node2D
{
    public RandomNumberGenerator Rng { get; set; }
    public int LeftIndex { get; set; }

    [Export]
    public Polyomino CurPolyomino { get; private set; }
    [Export]
    public Polyomino NextPolyomino { get; private set; }
    [Export]
    public BlockBoard BlockBoard { get; private set; }

    public Vector2I BoardSize { get; init; } = new Vector2I(10, 6); // Some top rows are for overflow blocks
    public int OverflowFrom { get; init; } = 3;

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
