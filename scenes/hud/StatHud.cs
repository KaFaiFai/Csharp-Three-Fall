using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class StatHud : Control
{
    public int Turn { get; private set; } = 0;
    public int Phase { get; private set; } = 0;
    public int Combo { get; private set; } = 0;

    private List<(int turn, int phase, int numRemoved, BlockType)> BlockTypeCombos { get; set; } = new();

    [Export]
    private Label ScoreLabel { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        UpdateLabel();
    }

    public void OnNewTurn()
    {
        Turn++;
        Phase = 0;
        Combo = 0;
        UpdateLabel();
    }

    public void OnPhase(int phase)
    {
        Phase = phase;
        UpdateLabel();
    }

    public void OnNewCombo(int numRemoved, BlockType type)
    {
        Combo++;
        BlockTypeCombos.Add((Turn, Phase, numRemoved, type));
        UpdateLabel();
    }

    private int CountNumBlocksRemoved()
    {
        return BlockTypeCombos.Aggregate(0, (value, combo) => value + combo.numRemoved);
    }

    private int FindMaxComboInTurn()
    {
        return BlockTypeCombos
            .GroupBy(combo => combo.turn)
            .Aggregate(0, (value, combo) => Math.Max(value, combo.Count()));
    }

    private int CalculateScore()
    {
        // TODO: better formula
        return CountNumBlocksRemoved();
    }

    private void UpdateLabel()
    {
        ScoreLabel.Text = $"Score: {CalculateScore()}";
    }
}
