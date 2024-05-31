using Godot;
using System;

public partial class Block : Node
{
	public BlockType Type { get; set; } = BlockType.d;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ColorRect colorRect = GetNode<ColorRect>("ColorRect");
		colorRect.Color = Type switch
		{
			BlockType.a => Colors.Blue,
			BlockType.b => Colors.Violet,
			BlockType.c => Colors.Yellow,
			BlockType.d => Colors.Green,
			_ => Colors.Black,
		};

		InputEvents inputEvents = GetNode<InputEvents>("InputEvents");
		GD.Print(inputEvents.GetSignalList());
		inputEvents.MovedLeft += OnMovedLeft;
		inputEvents.MovedRight += OnMovedRight;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnMovedLeft()
	{
		GD.Print("OnMovedLeft");
	}
	private void OnMovedRight()
	{
		GD.Print("OnMovedRight");
	}
}
