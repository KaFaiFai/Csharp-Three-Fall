using Godot;
using System;

public partial class GameScreen : Node2D
{
    public RandomNumberGenerator Rng { get; set; }
    public int LeftIndex { get; set; } = 0;

    private Polyomino _curPolyomino { get; set; }
    private Polyomino _nextPolyomino { get; set; }
    private BlockBoard _blockBoard { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Rng = new RandomNumberGenerator() { Seed = 0 };

        _curPolyomino = GetNode<Polyomino>("CurrentPolyomino");
        _nextPolyomino = GetNode<Polyomino>("NextPolyomino");
        _blockBoard = GetNode<BlockBoard>("BlockBoard");

        _blockBoard.BlockGrid.UpdateBlocksFromTypes(new BlockType?[10, 6]);
        AdvancePolyomino();
        AdvancePolyomino();
    }

    public void MoveLeft()
    {
        LeftIndex--;
    }

    public void MoveRight()
    {
        LeftIndex++;
    }

    private void AdvancePolyomino()
    {
        _curPolyomino.BlockGrid.UpdateBlocksFromTypes(_nextPolyomino.BlockGrid.ToBlockTypes());

        // TODO: Randomize shapes
        bool[,] shape = new bool[,] { { true, false }, { true, true } };
        BlockType?[,] newBlockTypes = new BlockType?[shape.GetLength(0), shape.GetLength(1)];
        BlockType[] allBlockTypes = Enum.GetValues<BlockType>();
        for (int i = 0; i < shape.GetLength(0); i++)
        {
            for (int j = 0; j < shape.GetLength(1); j++)
            {
                if (shape[i, j])
                {
                    newBlockTypes[i, j] = allBlockTypes[Rng.RandiRange(0, allBlockTypes.Length)];
                }
            }
        }
        _nextPolyomino.BlockGrid.UpdateBlocksFromTypes(newBlockTypes);
    }
}
