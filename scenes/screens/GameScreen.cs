using Godot;
using System;

public partial class GameScreen : Node2D
{
    public RandomNumberGenerator Rng { get; set; }
    public int LeftIndex { get; set; } = 0;
    public bool IsPlacingDisabled { get; set; } = false;

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

        AdvancePolyomino();
        AdvancePolyomino();
        _blockBoard.BlockGrid.UpdateBlocksFromTypes(new BlockType?[10, 6]);
        _blockBoard.UpdatePreviewPolyomino(_curPolyomino, LeftIndex);
    }

    public void MoveLeft()
    {
        LeftIndex--;
        _blockBoard.UpdatePreviewPolyomino(_curPolyomino, LeftIndex);
    }

    public void MoveRight()
    {
        LeftIndex++;
        _blockBoard.UpdatePreviewPolyomino(_curPolyomino, LeftIndex);
    }

    public async void RotateClockwise()
    {
        await _curPolyomino.RotateClockwise();
        _blockBoard.UpdatePreviewPolyomino(_curPolyomino, LeftIndex);
    }

    public async void RotateAnticlockwise()
    {
        await _curPolyomino.RotateAnticlockwise();
        GD.Print("Update preview");
        _blockBoard.UpdatePreviewPolyomino(_curPolyomino, LeftIndex);
    }

    public async void PlaceCurrentPolyomino()
    {
        if (!IsPlacingDisabled)
        {
            _blockBoard.ClearPreview();
            _blockBoard.ResolveBoardForNewPolyomino(_curPolyomino, LeftIndex);
            _curPolyomino.BlockGrid.UpdateBlocksFromTypes(new BlockType?[0, 0]);
            IsPlacingDisabled = true;
            await ToSignal(_blockBoard, BlockBoard.SignalName.BoardResolved);
            IsPlacingDisabled = false;
            AdvancePolyomino();
            _blockBoard.UpdatePreviewPolyomino(_curPolyomino, LeftIndex);
        }
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
