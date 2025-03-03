using System.ComponentModel.DataAnnotations;
using Godot;

public partial class Player2 : Node2D
{
	[Export] public int Speed = 100; 
	private Vector2 targetPosition = new Vector2(640, 200); 
	private bool isWalking = true;
	private bool phoneCallStarted = false;

	private AnimatedSprite2D animatedSprite;
	private float initialY; 

	
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		initialY = Position.Y; 

		animatedSprite.Play("walk");
		GetTree().CreateTimer(20).Timeout += StartPhoneRing; 
	}

	public override void _Process(double delta)
	{
		if (isWalking)
		{
			Vector2 direction = (targetPosition - Position).Normalized();
			Position = new Vector2(Position.X + direction.X * Speed * (float)delta, initialY); 

			if (Mathf.Abs(Position.X - targetPosition.X) < 5) 
			{
				isWalking = false;
				animatedSprite.Play("idle");
			}
		}
	}

	private void StartPhoneRing()
	{
		if (!phoneCallStarted)
		{
			phoneCallStarted = true;
			{
				animatedSprite.Play("ring"); 
			
				GetTree().CreateTimer(5).Timeout += StartPhoneCall;
			};
		}
	}

	private void  StartPhoneCall()
	{

		animatedSprite.Play("phone call");
		Panel panel = GetNodeOrNull<Panel>("/root/cutscene/Panel");
		if (panel != null)
		{
			panel.Visible = true;
		}

		GetTree().CreateTimer(5).Timeout += PlayerStartsTalking;
	}
	private void  PlayerStartsTalking()
	{
		animatedSprite.Play("phone call");
	}
	public void PlayAnimation(string animationName)
{
	if (animatedSprite.SpriteFrames.HasAnimation(animationName))
	{
		animatedSprite.Play(animationName);
	}
	
}

}
