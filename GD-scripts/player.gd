extends Node2D

@export var speed: int = 50
var target_position: Vector2 = Vector2(375, 200)
var is_walking: bool = true
var phone_call_started: bool = false

@onready var animated_sprite: AnimatedSprite2D = $AnimatedSprite2D
var initial_y: float

func _ready():
    initial_y = position.y

    animated_sprite.play("Walk")
    get_tree().create_timer(10).timeout.connect(start_phone_ring)

func _process(delta):
    if is_walking:
        var direction = (target_position - position).normalized()
        position = Vector2(position.x + direction.x * speed * delta, initial_y)

        if abs(position.x - target_position.x) < 5:  # Stop at target X
            is_walking = false
            animated_sprite.play("Phone Ring")

func start_phone_ring():
    if not phone_call_started:
        phone_call_started = true
        animated_sprite.play("Phone Ring")
        get_tree().create_timer(3).timeout.connect(start_phone_call)

func start_phone_call():
    animated_sprite.play("Phone call Listen")
    
    var panel = get_node_or_null("/root/cutscene/Panel")
    if panel:
        panel.visible = true

    get_tree().create_timer(5).timeout.connect(player_starts_talking)

func player_starts_talking():
    animated_sprite.play("Phone Call Talk")

func play_animation(animation_name: String):
    if animated_sprite.sprite_frames.has_animation(animation_name):
        animated_sprite.play(animation_name)

