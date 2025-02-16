using Godot;
using System;
using System.Linq.Expressions;

public partial class CharacterController : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	AnimatedSprite2D  charAnim;

	public override void _Ready(){
		charAnim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
			if(velocity.Y > 0){
				charAnim.Play("falling");
			}else if(velocity.Y < 0){
				charAnim.Play("jump");
			}
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("Move_Left", "Move_Right", "ui_up", "ui_down");
		if(direction.X > 0 && IsOnFloor()){
			Scale = new Vector2(4,4);
			RotationDegrees = 0;
		} else if(direction.X < 0 && IsOnFloor()){
			Scale = new Vector2(4,-4);
			RotationDegrees = 180;
		}
		
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
			if(IsOnFloor()){
				charAnim.Play("walk");
			}
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			if(IsOnFloor()){
				charAnim.Play("Idle");
			}
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
