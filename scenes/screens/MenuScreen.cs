using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class MenuScreen : Control
{
    [Export] public TextureRect ArrowLeft { get; set; }
    [Export] public TextureRect ArrowRight { get; set; }

    public Array<Button> MenuItems { get; set; }
    private PackedScene gameScene = GD.Load<PackedScene>("res://scenes/screens/GameScreen.tscn");

    public int? ItemIndex { get; private set; } = null;
    private float Margin { get; set; } = 20;

    public override void _Ready()
    {
        MenuItems = new Array<Button>
        {
            GetNode<Button>("MenuItems/SinglePlayer"),
            GetNode<Button>("MenuItems/Cooperative"),
            GetNode<Button>("MenuItems/Competitive"),
            GetNode<Button>("MenuItems/Exit"),
        };

        for (int i = 0; i < MenuItems.Count; i++)
        {
            int i_ = i; // stupid C# closure
            MenuItems[i].MouseEntered += () => ChangeItemIndex(i_);
            MenuItems[i].MouseExited += () => ChangeItemIndex(null);
            MenuItems[i].Pressed += OnSelected;
        }

        ChangeItemIndex(null);
    }

    private void ChangeItemIndex(int? index)
    {
        ItemIndex = index;
        CallDeferred(MethodName.UpdateArrowsPosition);
    }

    private void OnSelected()
    {
        if (ItemIndex == null) return;
        switch (MenuItems[(int)ItemIndex].Name)
        {
            case "SinglePlayer":
            case "Cooperative":
            case "Competitive":
                GameScreen gameScreen = gameScene.Instantiate<GameScreen>();
                gameScreen.GameMode = MenuItems[(int)ItemIndex].Name;
                //PackedScene packedScene = new PackedScene();
                //packedScene.Pack(gameScreen);
                GetTree().Root.AddChild(gameScreen);
                QueueFree();
                break;
            case "Exit":
                GetTree().Quit();
                break;
        }
    }

    private void UpdateArrowsPosition()
    {
        if (ItemIndex == null)
        {
            ArrowLeft.Visible = false;
            ArrowRight.Visible = false;
            return;
        }

        ArrowLeft.Visible = true;
        ArrowRight.Visible = true;
        Control item = MenuItems[(int)ItemIndex];
        Vector2 itemSize = item.Size * item.Scale;
        Vector2 leftSize = ArrowLeft.Size * ArrowLeft.Scale;
        Vector2 rightSize = ArrowRight.Size * ArrowRight.Scale;

        float leftX = item.GlobalPosition.X - leftSize.X - Margin;
        float leftY = item.GlobalPosition.Y + (itemSize.Y - leftSize.Y) / 2;
        ArrowLeft.GlobalPosition = new Vector2(leftX, leftY);

        float rightX = item.GlobalPosition.X + itemSize.X + Margin;
        float rightY = item.GlobalPosition.Y + (itemSize.Y - rightSize.Y) / 2;
        ArrowRight.GlobalPosition = new Vector2(rightX, rightY);
    }
}
