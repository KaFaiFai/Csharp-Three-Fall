using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerStateMachine : Node
{
    [Export]
    public PlayerState InitialState { get; set; }

    public PlayerState CurrentState { get; private set; }
    public Dictionary<String, PlayerState> States { get; private set; } = new();

    public override void _Ready()
    {
        foreach (var child in GetChildren())
        {
            if (child is PlayerState)
            {
                PlayerState state = (PlayerState)child;
                States[child.Name] = state;
                state.Transitioned += OnStateTransitioned;
            }
        }

        InitialState.OnEnter();
        CurrentState = InitialState;
    }

    private void OnStateTransitioned(PlayerState newState)
    {
        CurrentState.OnExit();
        newState.OnEnter();
        CurrentState = newState;
        GD.Print($"Entered new state {newState.Name}");
    }
}
