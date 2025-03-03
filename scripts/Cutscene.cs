using Godot;
using System.Threading.Tasks;

public partial class Cutscene : Node2D
{
	private ColorRect fadeOverlay;

	public override void _Ready()
	{
		// Get the fade overlay
		fadeOverlay = GetNode<ColorRect>("CanvasLayer/FadeOverlay");

		// Ensure it starts fully black
		if (fadeOverlay != null)
		{
			fadeOverlay.Modulate = new Color(0, 0, 0, 1);
			StartFadeIn();
		}
	}

	private async void StartFadeIn()
	{
		if (fadeOverlay != null)
		{
			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(fadeOverlay, "modulate:a", 0.0f, 1.5f); // Fade from black in 2.5 seconds
			await ToSignal(tween, "finished");

			// Optional: Remove fade overlay after fade-in is complete
			fadeOverlay.QueueFree();
		}
	}
}
