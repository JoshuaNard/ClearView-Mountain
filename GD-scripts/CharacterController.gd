extends CharacterBody2D

const SPEED = 325.0
const MIN_JUMP_VELOCITY = -400.0
const MAX_JUMP_VELOCITY = -800.0
var gravity = 980.0

@onready var char_anim = $AnimatedSprite2D
@onready var jump_progress_bar = $JumpProgress
@onready var snow = $Snow if has_node("Snow") else null

var respawn_point = Vector2.ZERO
var jump_progress = 0.0
var max_jump_progress = 100.0
var fill_rate = 50.0
var is_jumping = false
var snowing = true
var has_gold_boots = false  # Tracks whether the player has gold boots

func _ready():
	jump_progress_bar.visible = false
	jump_progress_bar.value = 0
	add_to_group("player")
	
	var death_barrier = get_tree().get_first_node_in_group("DeathBarrier")
	var anti_snow = get_tree().get_first_node_in_group("antiSnowBarrier")
	var snow_barrier = get_tree().get_first_node_in_group("SnowBarrier")
	
	if death_barrier:
		death_barrier.connect("body_entered", _on_death_zone_entered)
	if snow_barrier:
		snow_barrier.connect("body_entered", _on_snow_barrier_entered)
	if anti_snow:
		anti_snow.connect("body_entered", _on_anti_snow_barrier_entered)
	
	print("Player added to group: player")

func _physics_process(delta):
	# Use the existing velocity property
	velocity.y += gravity * delta if not is_on_floor() else 0
	
	# Apply gravity
	if not is_on_floor():
		if velocity.y > 0:
			play_animation("falling")
		elif velocity.y < 0 and char_anim.animation != "postJump":
			play_animation("postJump")
		if velocity.y < 0 and not char_anim.is_playing():
			char_anim.stop()
			char_anim.frame = char_anim.sprite_frames.get_frame_count("postJump") - 1

	# Handle Jump Charge
	if Input.is_action_pressed("ui_accept") and is_on_floor():
		if jump_progress > 5.0:
			jump_progress_bar.visible = true
		jump_progress = clamp(jump_progress + fill_rate * delta, 0, max_jump_progress)
		jump_progress_bar.value = jump_progress
		is_jumping = true
		if velocity.x == 0:
			play_animation("jump")
	else:
		jump_progress_bar.visible = false

	# Execute Jump
	if Input.is_action_just_released("ui_accept") and is_on_floor():
		var jump_strength = lerp(MIN_JUMP_VELOCITY, MAX_JUMP_VELOCITY, jump_progress / max_jump_progress)
		velocity.y = jump_strength
		jump_progress = 0
		jump_progress_bar.value = 0
		is_jumping = false

	# Handle Movement
	var direction = Input.get_vector("Move_Left", "Move_Right", "ui_up", "ui_down")
	
	# Rotate sprite based on movement direction
	if direction.x > 0:
		scale = Vector2(4, 4)
		rotation_degrees = 0
	elif direction.x < 0:
		scale = Vector2(4, -4)
		rotation_degrees = 180

	# Movement logic
	if direction != Vector2.ZERO:
		velocity.x = direction.x * SPEED
		if is_on_floor():
			play_animation("walk")
	else:
		velocity.x = move_toward(velocity.x, 0, SPEED)
		if is_on_floor() and velocity.x == 0 and not is_jumping:
			play_animation("Idle")

	move_and_slide()


func play_animation(animation_name):
	if not char_anim:
		return
	
	# If player has gold boots, use gold boot versions of animations
	if has_gold_boots:
		animation_name = "Gold" + animation_name
		gravity = 780.0

	if char_anim.animation != animation_name:
		char_anim.play(animation_name)

func _on_death_zone_entered(body):
	if body == self:
		call_deferred("die_and_reset")

func _on_snow_barrier_entered(body):
	if body == self:
		print("Entered SnowBarrier")
		call_deferred("toggle_snow")

func _on_anti_snow_barrier_entered(body):
	if body == self:
		print("Entered antiSnowBarrier")
		call_deferred("toggle_snow")

func toggle_snow():
	snowing = !snowing
	if snow:
		snow.emitting = snowing
	print("Snowing:", snowing)

func die_and_reset():
	if respawn_point != Vector2.ZERO:
		global_position = respawn_point
	else:
		get_tree().reload_current_scene()

func set_respawn_point(new_respawn_point):
	respawn_point = new_respawn_point

func set_gold_boots(value):
	has_gold_boots = value
