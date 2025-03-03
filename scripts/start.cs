using Godot;
using System;

public partial class start : Control
{
	private Button playButton;
	private ColorRect fadeOverlay;

	[Export] public string GameScenePath = "res://GameJam/cutscene.tscn"; 

	public override void _Ready()
	{
		playButton = GetNode<Button>("Button");
		fadeOverlay = GetNode<ColorRect>("FadeOverlay");
		playButton.Pressed += OnPlayButtonPressed;
	}

	private async void OnPlayButtonPressed()
	{
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(fadeOverlay, "modulate:a", 1.0f, 1.5f); 

		await ToSignal(GetTree().CreateTimer(1.5f), "timeout"); 
		GetTree().ChangeSceneToFile(GameScenePath);
	}
}
