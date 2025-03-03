using Godot;

public partial class Torch : Node2D
{
	private AnimatedSprite2D torch;
	public override void _Ready()
{
	torch = GetNode<AnimatedSprite2D>("torchAnim");
	torch.Play("lit");
}}
