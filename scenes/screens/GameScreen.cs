using Godot;
using System;
using System.Threading.Tasks;

public partial class GameScreen : Node2D
{
    [Signal]
    public delegate void EnteredNewTurnEventHandler();

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
        WallKick();
        _blockBoard.UpdatePreviewPolyomino(_curPolyomino, LeftIndex);

        EmitSignal(SignalName.EnteredNewTurn);
    }

    public void MoveLeft()
    {
        LeftIndex--;
        WallKick();
        _blockBoard.UpdatePreviewPolyomino(_curPolyomino, LeftIndex);
    }

    public void MoveRight()
    {
        LeftIndex++;
        WallKick();
        _blockBoard.UpdatePreviewPolyomino(_curPolyomino, LeftIndex);
    }

    public async void RotateClockwise()
    {
        var task = _curPolyomino.Rotate(clockwise: true);
        WallKick();
        _blockBoard.UpdatePreviewPolyomino(_curPolyomino, LeftIndex);
        await task;
    }

    public async void RotateAnticlockwise()
    {
        var task = _curPolyomino.Rotate(clockwise: false);
        WallKick();
        _blockBoard.UpdatePreviewPolyomino(_curPolyomino, LeftIndex);
        await task;
    }

    public async void PlaceCurrentPolyomino()
    {
        if (!IsPlacingDisabled)
        {
            _blockBoard.ClearPreview();
            var task = _blockBoard.ResolveBoardForNewPolyomino(_curPolyomino, LeftIndex);
            _curPolyomino.BlockGrid.UpdateBlocksFromTypes(new BlockType?[0, 0]);
            IsPlacingDisabled = true;
            await task;

            IsPlacingDisabled = false;
            AdvancePolyomino();
            WallKick();
            _blockBoard.UpdatePreviewPolyomino(_curPolyomino, LeftIndex);
            EmitSignal(SignalName.EnteredNewTurn);
        }
    }

    private void AdvancePolyomino()
    {
        _curPolyomino.BlockGrid.UpdateBlocksFromTypes(_nextPolyomino.BlockGrid.ToBlockTypes());

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
        _nextPolyomino.BlockGrid.UpdateBlocksFromTypes(newBlockTypes);
    }

    private void WallKick()
    {
        int baordWidth = _blockBoard.BlockGrid.Blocks.GetLength(1);
        int blocksWidth = _curPolyomino.BlockGrid.Blocks.GetLength(1);
        LeftIndex = Mechanics.WallKick(baordWidth, blocksWidth, LeftIndex);
    }
}
