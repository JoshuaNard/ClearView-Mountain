using Godot;
using System;

public partial class Campfire : Area2D
{
	private AnimatedSprite2D fireAnimation;
	private Light2D campfireLight;
	private GpuParticles2D embers;
	private GpuParticles2D smoke;
	private bool isLit = false;

	public override void _Ready()
	{
		// Get nodes
		fireAnimation = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
		campfireLight = GetNodeOrNull<Light2D>("Light2D");
		embers = GetNodeOrNull<GpuParticles2D>("Embers");
		smoke = GetNodeOrNull<GpuParticles2D>("Smoke");

		// Ensure nodes exist
		if (fireAnimation == null || campfireLight == null || embers == null || smoke == null)
		{
			GD.PrintErr("Campfire: Missing required nodes! Check scene hierarchy.");
			return;
		}

		// Default state: Unlit fire
		fireAnimation.Play("unlit fire");  // Play the unlit fire animation
		campfireLight.Visible = false;     // Light is OFF
		embers.Emitting = false;           // Embers OFF
		smoke.Emitting = true;             // Smoke ON

		BodyEntered += OnPlayerEntered;
	}

	private void OnPlayerEntered(Node body)
	{
		if (body is CharacterController player)
		{
			if (player == null)
			{
				GD.PrintErr("Campfire: Player reference is null!");
				return;
			}

			// Set player's respawn point
			player.SetRespawnPoint(GlobalPosition);

			// Light the campfire if it's not already lit
			if (!isLit)
			{
				isLit = true;
				fireAnimation.Play("burn");   // Switch to fire animation
				campfireLight.Visible = true; // Show light
				embers.Emitting = true;       // Start embers
				smoke.Emitting = false;       // Stop smoke
				FlickerLight();               // Make light flicker
			}
		}
	}

	private void FlickerLight()
	{
		var tween = GetTree().CreateTween().SetLoops();
		tween.TweenProperty(campfireLight, "energy", 1.2f, (float)GD.RandRange(0.1, 0.3));
		tween.TweenProperty(campfireLight, "energy", 2.0f, (float)GD.RandRange(0.1, 0.3));
	}
}
