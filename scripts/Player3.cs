using Godot;

public partial class Player3 : Node2D
{
    [Export] public int Speed = 50; 
    private Vector2 targetPosition = new Vector2(350, 200); 
    private bool isWalking = true;

    private AnimatedSprite2D animatedSprite;
    private float initialY; // Store initial Y position

    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        initialY = Position.Y; // Store initial Y position

        animatedSprite.Play("Walk");
    }

    public override void _Process(double delta)
    {
        if (isWalking)
        {
            Vector2 direction = (targetPosition - Position).Normalized();
            Position = new Vector2(Position.X + direction.X * Speed * (float)delta, initialY); 

            if (Mathf.Abs(Position.X - targetPosition.X) < 5) // Stop at target X
            {
                isWalking = false;
                animatedSprite.Play("Idle"); 
              
            }
        }
    }

    public void PlayAnimation(string animationName)
    {
        if (animatedSprite.SpriteFrames.HasAnimation(animationName))
        {
            animatedSprite.Play(animationName);
        }
    }
}
