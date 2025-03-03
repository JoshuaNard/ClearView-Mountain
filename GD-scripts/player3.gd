extends Node2D

@export var speed: int = 50
var target_position: Vector2 = Vector2(350, 200)
var is_walking: bool = true

@onready var animated_sprite: AnimatedSprite2D = $AnimatedSprite2D
var initial_y: float

func _ready():
	initial_y = position.y
	animated_sprite.play("Walk")

func _process(delta):
	if is_walking:
		move_towards_target(delta)

func move_towards_target(delta):
	var direction = (target_position - position).normalized()
	position.x += direction.x * speed * delta

	if abs(position.x - target_position.x) < 5:  # Stop at target X
		is_walking = false
		animated_sprite.play("Idle")

func play_animation(animation_name: String):
	if animated_sprite and animated_sprite.sprite_frames.has_animation(animation_name):
		animated_sprite.play(animation_name)
