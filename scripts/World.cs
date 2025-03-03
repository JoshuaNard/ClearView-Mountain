using Godot;
using System.Threading.Tasks;

public partial class World : Node2D
{
	private ColorRect fadeOverlay;
public override void _Ready()
{
    fadeOverlay = GetNode<ColorRect>("CanvasLayer/FadeOverlay");

    if (fadeOverlay != null)
    {
        fadeOverlay.Visible = true; // Make sure it's visible at start
        fadeOverlay.Modulate = new Color(0, 0, 0, 1); // Start as fully black
        StartFadeIn();
    }
	}

	private async void StartFadeIn()
	{
		if (fadeOverlay != null)
		{
			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(fadeOverlay, "modulate:a", 0.0f, 2.5f); // Fade from black in 2.5 seconds
			await ToSignal(tween, "finished");

			fadeOverlay.Visible = false;
		}
	}
}
