extends Node2D

@export var speed: int = 100
var target_position: Vector2 = Vector2(630, 200)
var is_walking: bool = true
var phone_call_started: bool = false

@onready var animated_sprite: AnimatedSprite2D = $AnimatedSprite2D
var initial_y: float

func _ready():
	initial_y = position.y
	animated_sprite.play("walk")

	# Start phone ring event after 20 seconds
	get_tree().create_timer(20).timeout.connect(start_phone_ring)

func _process(delta):
	if is_walking:
		var direction = (target_position - position).normalized()
		position = Vector2(position.x + direction.x * speed * delta, initial_y)

		if abs(position.x - target_position.x) < 5:
			is_walking = false
			animated_sprite.play("idle")

func start_phone_ring():
	if not phone_call_started:
		phone_call_started = true
		animated_sprite.play("ring")

		# Start phone call after 5 seconds
		get_tree().create_timer(5).timeout.connect(start_phone_call)

func start_phone_call():
	animated_sprite.play("phone call")

	var panel = get_node_or_null("/root/cutscene/Panel")
	if panel:
		panel.visible = true

	# Player starts talking after 5 seconds
	get_tree().create_timer(5).timeout.connect(player_starts_talking)

func player_starts_talking():
	animated_sprite.play("phone call")

func play_animation(animation_name: String):
	if animated_sprite.sprite_frames.has_animation(animation_name):
		animated_sprite.play(animation_name)
