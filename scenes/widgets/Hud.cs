using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Hud : Control
{
    public int Turn { get; private set; }
    public int Phase { get; private set; }

    /// <summary>
    /// Represents, in each combo, the turn, the phase, the number of blocks removed and their blocktype
    /// </summary>
    private List<(int, int, int, BlockType)> BlockTypeCombos { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void OnNewTurn()
    {
        Turn++;
        Phase = 0;
    }

    public void OnPhase(int phase)
    {
        Phase = phase;
    }

    public void OnNewCombo(int numRemoved, BlockType type)
    {
        BlockTypeCombos.Add((Turn, Phase, numRemoved, type));
    }

    public int CountNumBlocksRemoved()
    {
        return BlockTypeCombos.Aggregate(0, (value, combo) => value + combo.Item2);
    }

    public int FindMaxComboInTurn()
    {
        return BlockTypeCombos.GroupBy(combo => combo.Item1).Select(a => a.Count()).Max();
    }

    public int CalculateScore()
    {
        // TODO: better formula
        return CountNumBlocksRemoved();
    }

}
