using Godot;
using System;

public partial class CharacterController : CharacterBody2D
{
	public const float Speed = 325.0f;
	public const float MinJumpVelocity = -400.0f;
	public const float MaxJumpVelocity = -800.0f;
	private float Gravity = 980f;
	private GpuParticles2D Snow;

	private Vector2 respawnPoint = Vector2.Zero;

	private TextureProgressBar JumpProgressBar;
	public float JumpProgress = 0f;
	public float MaxJumpProgress = 100f;
	public float FillRate = 50f;
	bool isJumping = false;
	bool snowing = true;

	private AnimatedSprite2D charAnim;
	public bool hasGoldBoots = false; // NEW: Tracks whether the player has gold boots

	public override void _Ready()
	{
		charAnim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		JumpProgressBar = GetNode<TextureProgressBar>("JumpProgress");
		Snow = GetNodeOrNull<GpuParticles2D>("Snow");
		Area2D deathBarrier = GetTree().GetFirstNodeInGroup("DeathBarrier") as Area2D;
		Area2D antiSnow = GetTree().GetFirstNodeInGroup("antiSnowBarrier") as Area2D;
		Area2D snowBarrier = GetTree().GetFirstNodeInGroup("SnowBarrier") as Area2D;
		JumpProgressBar.Visible = false;
		JumpProgressBar.Value = 0;

		GD.Print("Player added to group: player");
		AddToGroup("player");

		if (deathBarrier != null)
		{
			deathBarrier.BodyEntered += OnDeathZoneEntered;
		}
		if (snowBarrier != null)
		{
			snowBarrier.BodyEntered += isSnowing;
		}
		if (antiSnow != null)
		{
			antiSnow.BodyEntered += antiSnowing;
		}


	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add gravity
		if (!IsOnFloor())
		{
			velocity.Y += Gravity * (float)delta;
			if (velocity.Y > 0)
			{
				PlayAnimation("falling");
			}
			else if (velocity.Y < 0 && charAnim.Animation != "postJump")
			{
				PlayAnimation("postJump");
			}
			if (velocity.Y < 0 && !charAnim.IsPlaying())
			{
				charAnim.Stop();
				charAnim.Frame = charAnim.SpriteFrames.GetFrameCount("postJump") - 1;
			}
		}

		// Handle Jump
		if (Input.IsActionPressed("ui_accept") && IsOnFloor())
		{
			if (JumpProgress > 5f)
			{
				JumpProgressBar.Visible = true;
			}

			JumpProgress = Mathf.Clamp(JumpProgress + FillRate * (float)delta, 0, MaxJumpProgress);
			JumpProgressBar.Value = JumpProgress;

			isJumping = true;
			if (velocity.X == 0)
			{
				PlayAnimation("jump");
			}
		}
		else
		{
			JumpProgressBar.Visible = false;
		}

		if (Input.IsActionJustReleased("ui_accept") && IsOnFloor())
		{
			float jumpStrength = Mathf.Lerp(MinJumpVelocity, MaxJumpVelocity, JumpProgress / MaxJumpProgress);
			velocity.Y = jumpStrength;

			JumpProgress = 0;
			JumpProgressBar.Value = 0;
			isJumping = false;
		}

		// Get the input direction and handle movement
		Vector2 direction = Input.GetVector("Move_Left", "Move_Right", "ui_up", "ui_down");

		// Rotate the sprite so it faces the right way based on user input
		if (direction.X > 0)
		{
			Scale = new Vector2(4, 4);
			RotationDegrees = 0;
		}
		else if (direction.X < 0)
		{
			Scale = new Vector2(4, -4);
			RotationDegrees = 180;
		}

		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
			if (IsOnFloor())
			{
				PlayAnimation("walk");
			}
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			if (IsOnFloor() && velocity.X == 0 && !isJumping)
			{
				PlayAnimation("Idle");
			}
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	private void PlayAnimation(string animationName)
	{
		if (charAnim == null) return;

		// If player has gold boots, use gold boot versions of animations
		if (hasGoldBoots)
		{
			animationName = "Gold" + animationName;
			Gravity = 780f;
		}

		if (charAnim.Animation != animationName)
		{
			charAnim.Play(animationName);
		}
	}

	private void OnDeathZoneEntered(Node body)
	{
		if (body == this)
		{
			CallDeferred(nameof(DieAndReset));
		}
	}
	private void isSnowing(Node body)
	{
		if (body == this)
		{
			GD.Print("Entered SnowBarrier");
			CallDeferred(nameof(setSnow));
		}
	}

	private void antiSnowing(Node body)
	{
		if (body == this)
		{
			GD.Print("Entered antiSnowBarrier");
			CallDeferred(nameof(setSnow));
		}
	}

	private void setSnow(){
		snowing = !snowing;
		Snow.Emitting = snowing;
		GD.Print("snowing");
	}

	private void DieAndReset()
	{
		if (respawnPoint != Vector2.Zero)
		{
			GlobalPosition = respawnPoint;
		}
		else
		{
			GetTree().ReloadCurrentScene();
		}
	}

	public void SetRespawnPoint(Vector2 newRespawnPoint)
	{
		respawnPoint = newRespawnPoint;
	}

	public void SetGoldBoots(bool value)
	{
		hasGoldBoots = value;
	}
}
