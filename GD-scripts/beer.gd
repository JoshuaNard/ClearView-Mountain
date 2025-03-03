extends Area2D

var player_nearby = false
var interactable = true

@onready var char_anim = $AnimatedSprite2D

func _ready():
    char_anim.play("default")
    connect("body_entered", _on_player_entered)
    connect("body_exited", _on_player_exited)

func _on_player_entered(body):
    if body.is_in_group("player"):
        player_nearby = true

func _on_player_exited(body):
    if body.is_in_group("player"):
        player_nearby = false

func _process(delta):
    if player_nearby and Input.is_action_just_pressed("interact"):
        queue_free()  # Remove the beer bottle from the scene

