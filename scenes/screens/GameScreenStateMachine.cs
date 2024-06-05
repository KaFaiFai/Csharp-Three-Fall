using Godot;
using System;
using System.Collections.Generic;

public partial class GameScreenStateMachine : Node
{
    [Export]
    public GameScreenState InitialState { get; set; }

    public GameScreenState CurrentState { get; private set; }
    public Dictionary<String, GameScreenState> States { get; private set; } = new();

    public override void _Ready()
    {
        foreach (var child in GetChildren())
        {
            if (child is GameScreenState)
            {
                GameScreenState state = (GameScreenState)child;
                States[child.Name] = state;
                state.Transitioned += OnStateTransitioned;
            }
        }

        InitialState.OnEnter();
        CurrentState = InitialState;
    }

    private void OnStateTransitioned(String newStateName)
    {
        if (!States.ContainsKey(newStateName)) throw new ArgumentException($"Invalid state name {newStateName}");

        GameScreenState newState = States[newStateName];
        CurrentState.OnExit();
        newState.OnEnter();
        CurrentState = newState;
        GD.Print($"Entered new state {newStateName}");
    }
}
