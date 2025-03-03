using Godot;

public partial class Wife : Node2D
{
	private AnimatedSprite2D wife;
	public override void _Ready()
{
	wife = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	wife.Play("default");
}}
