using Godot;

public partial class beer : Area2D
{
	private bool playerNearby = false;
	private bool interactable = true;

	private AnimatedSprite2D charAnim;

	public override void _Ready()
	{
		charAnim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		charAnim.Play("default");
		Connect("body_entered", new Callable(this, nameof(OnPlayerEntered)));
		Connect("body_exited", new Callable(this, nameof(OnPlayerExited)));
		
	}

	private void OnPlayerEntered(Node body)
	{
		if (body.IsInGroup("player"))
		{
			playerNearby = true;
		}
	}

	private void OnPlayerExited(Node body)
	{
		if (body.IsInGroup("player"))
		{
			playerNearby = false;
		}
	}

	public override void _Process(double delta)
	{

		if (playerNearby && Input.IsActionJustPressed("interact"))
		{
			QueueFree(); // Remove the beer bottle from the scene
		}
	}
}
