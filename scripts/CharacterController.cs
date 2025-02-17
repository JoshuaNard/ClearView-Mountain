using Godot;
using System;
using System.Linq.Expressions;

public partial class CharacterController : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float MinJumpVelocity = -200.0f;
	public const float MaxJumpVelocty = -800.0f;
	private const float Gravity = 980f;
	
	private TextureProgressBar JumpProgressBar;
	public float JumpProgress = 0f;
	public float MaxJumpProgress = 100f;
	public float FillRate = 50f;
	bool isJumping = false;


	AnimatedSprite2D  charAnim;

	public override void _Ready(){
		charAnim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		JumpProgressBar = GetNode<TextureProgressBar>("JumpProgress");
		JumpProgressBar.Visible = false;
		JumpProgressBar.Value = 0;
	}

	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity.Y += Gravity * (float)delta;
			if(velocity.Y > 0 ){
				charAnim.Play("falling");
			}else if(velocity.Y < 0 && charAnim.Animation != "postJump"){
				charAnim.Play("postJump");
			}
			if (velocity.Y < 0 && !charAnim.IsPlaying())
			{
				charAnim.Stop();
				charAnim.Frame = charAnim.SpriteFrames.GetFrameCount("postJump") - 1;
			}
			
		}

		// Handle Jump.
		if (Input.IsActionPressed("ui_accept") && IsOnFloor())
		{
			
			JumpProgressBar.Visible = true;
			JumpProgress = Mathf.Clamp(JumpProgress + FillRate * (float)delta, 0, MaxJumpProgress);
			JumpProgressBar.Value = JumpProgress;
			
			//velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			isJumping = true;
			charAnim.Play("jump");
			
			
		}
		else{
			JumpProgressBar.Visible = false;
		}
		if (Input.IsActionJustReleased("ui_accept") && IsOnFloor()){
		
		float jumpStrength = Mathf.Lerp(MinJumpVelocity, MaxJumpVelocty, JumpProgress / MaxJumpProgress );
		velocity.Y = jumpStrength;
		
		JumpProgress = 0;
		JumpProgressBar.Value = 0;
		isJumping = false;
		}
		if (!Input.IsActionPressed("ui_accept")){
			// Get the input direction and handle the movement/deceleration.
			Vector2 direction = Input.GetVector("Move_Left", "Move_Right", "ui_up", "ui_down");
			
			// Rotate the sprite so it faces the right way based on users input
			if(direction.X > 0 && IsOnFloor()){
				Scale = new Vector2(5,5);
				RotationDegrees = 0;

				//face right
			} else if(direction.X < 0 && IsOnFloor()){
				Scale = new Vector2(5,-5);
				RotationDegrees = 180;
				//face left
			}
			
			if (direction != Vector2.Zero)
			{
				velocity.X = direction.X * Speed;
				if(IsOnFloor() && !isJumping){
					charAnim.Play("walk");
				}
			}
			else
			{
				velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
				if(IsOnFloor() && velocity.X == 0 && !isJumping){
				charAnim.Play("Idle");
				}
			}
		}
		else{
			velocity.X = 0;
		}
		Velocity = velocity;
		MoveAndSlide();
	}
}
