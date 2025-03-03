using Godot;
using System;

public partial class Mushroom : CharacterBody2D
{
	private bool playerNearby = false;
	private AnimatedSprite2D mushroomSprite1;
	private AnimatedSprite2D mushroomSprite2;
	private Light2D mushroomLight; // Reference to Light2D
	private GpuParticles2D mushroomParticles;
	private ColorRect fadeOverlay;


	public override void _Ready()
	{
		GetNode<Area2D>("Area2D").BodyEntered += OnPlayerEnter;
		GetNode<Area2D>("Area2D").BodyExited += OnPlayerExit;

		mushroomSprite1 = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		mushroomSprite2 = GetNode<AnimatedSprite2D>("AnimatedSprite2D2");
		mushroomLight = GetNodeOrNull<Light2D>("Light2D"); // Get Light2D (ensure it exists)
		mushroomParticles = GetNodeOrNull<GpuParticles2D>("flow");
		fadeOverlay = GetNodeOrNull<ColorRect>("CanvasLayer/FadeOverlay");

		// Ensure both sprites start playing their default animation
		if (mushroomSprite1 != null) mushroomSprite1.Play("default");
		if (mushroomSprite2 != null) mushroomSprite2.Play("default");
	}

	public override void _Process(double delta)
	{
		if (playerNearby && Input.IsActionJustPressed("interact")) // Press "E" to interact
		{
			InteractWithMushroom();
		}
	}

	private void OnPlayerEnter(Node body)
	{
		if (body.IsInGroup("player")) // Ensure it's the player
		{
			playerNearby = true;
		}
	}

	private void OnPlayerExit(Node body)
	{
		if (body.IsInGroup("player"))
		{
			playerNearby = false;
		}
	}

	private void InteractWithMushroom()
	{
		if (mushroomSprite1 != null) mushroomSprite1.Visible = false; // Hide first sprite
		if (mushroomSprite2 != null) mushroomSprite2.Visible = false; // Hide second sprite
		if (mushroomLight != null) mushroomLight.Visible = false; // Disable Light2D
		if (mushroomParticles != null) mushroomParticles.Visible = false;


		EndGameSequence();
	}

	
	private void EndGameSequence()
	{
		GetTree().ChangeSceneToFile("res://GameJam/Ending_Cutscene.tscn"); // Change to the new end screen
	}
}
