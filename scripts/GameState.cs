using Godot;
using System;

public partial class GameState : Node
{
    private static GameState _instance;
    public static GameState Instance { get { return _instance; } }

    public int HopeScore { get; private set; } = 16; // Default Hope Score

    public override void _Ready()
    {
        if (_instance == null)
        {
            _instance = this;
            SetProcess(true);
        }
        else
        {
            QueueFree(); // Prevent duplicates
        }
    }

    public void IncreaseHope(int amount)
    {
        HopeScore += amount;
    }

    public void DecreaseHope(int amount)
    {
        HopeScore -= amount;
    }
}


