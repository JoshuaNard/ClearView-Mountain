using Godot;
using System.Threading.Tasks;

public partial class EndScreen : ColorRect
{
	private ColorRect blackScreen;
	private Label thankYouLabel;

	public override void _Ready()
	{
		blackScreen = GetNode<ColorRect>("BlackScreen");
		thankYouLabel = GetNode<Label>("ThankYouLabel");

		// Start fully transparent
		blackScreen.Modulate = new Color(0, 0, 0, 0); // Invisible
		thankYouLabel.Modulate = new Color(1, 1, 1, 0); // Invisible

		// Start fade-in
		FadeIn();
	}

	private async void FadeIn()
	{
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(blackScreen, "modulate:a", 1.0f, 3.0f); // Fade in black screen
		await ToSignal(GetTree().CreateTimer(3.0f), "timeout"); // Wait for fade

		Tween textTween = GetTree().CreateTween();
		textTween.TweenProperty(thankYouLabel, "modulate:a", 1.0f, 2.5f); // Fade in text

		await ToSignal(GetTree().CreateTimer(5.0f), "timeout"); // Keep screen for 5 sec
		GetTree().Quit(); // Exit the game
		
	}
}
