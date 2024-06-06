using Godot;
using System;

public partial class PlayerHand : Node2D
{
    [Export] public ulong Seed { get; private set; }
    [Export] public Polyomino CurPolyomino { get; private set; }
    [Export] public Polyomino NextPolyomino { get; private set; }

    public int LeftIndex { get; set; }

    private RandomNumberGenerator Rng { get; set; }

    public override void _Ready()
    {
        Rng = new RandomNumberGenerator() { Seed = Seed };
        AdvancePolyomino();
        AdvancePolyomino();
        LeftIndex = 0;
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

    public void WallKick(BlockBoard blockBoard)
    {
        int baordWidth = blockBoard.BlockGrid.Blocks.GetLength(1);
        int blocksWidth = CurPolyomino.BlockGrid.Blocks.GetLength(1);
        LeftIndex = Mechanics.WallKick(baordWidth, blocksWidth, LeftIndex);
    }
}
