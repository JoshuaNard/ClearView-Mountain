extends CharacterBody2D

var player_nearby = false

@onready var mushroom_sprite1 = $AnimatedSprite2D
@onready var mushroom_sprite2 = $AnimatedSprite2D2
@onready var mushroom_light = $Light2D if has_node("Light2D") else null
@onready var mushroom_particles = $flow if has_node("flow") else null
@onready var fade_overlay = $CanvasLayer/FadeOverlay if has_node("CanvasLayer/FadeOverlay") else null
@onready var area_2d = $Area2D

func _ready():
    area_2d.connect("body_entered", _on_player_enter)
    area_2d.connect("body_exited", _on_player_exit)

    # Ensure both sprites start playing their default animation
    if mushroom_sprite1:
        mushroom_sprite1.play("default")
    if mushroom_sprite2:
        mushroom_sprite2.play("default")

func _process(delta):
    if player_nearby and Input.is_action_just_pressed("interact"): # Press "E" to interact
        interact_with_mushroom()

func _on_player_enter(body):
    if body.is_in_group("player"): # Ensure it's the player
        player_nearby = true

func _on_player_exit(body):
    if body.is_in_group("player"):
        player_nearby = false

func interact_with_mushroom():
    if mushroom_sprite1:
        mushroom_sprite1.visible = false # Hide first sprite
    if mushroom_sprite2:
        mushroom_sprite2.visible = false # Hide second sprite
    if mushroom_light:
        mushroom_light.visible = false # Disable Light2D
    if mushroom_particles:
        mushroom_particles.visible = false

    end_game_sequence()

func end_game_sequence():
    get_tree().change_scene_to_file("res://GameJam/Ending_Cutscene.tscn") # Change to the new end screen
