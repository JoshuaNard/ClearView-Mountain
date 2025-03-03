extends Control

@export var game_scene_path: String = "res://GameJam/cutscene.tscn"

@onready var play_button: Button = $Button
@onready var fade_overlay: ColorRect = $FadeOverlay

func _ready():
	play_button.pressed.connect(_on_play_button_pressed)

func _on_play_button_pressed():
	var tween = get_tree().create_tween()
	tween.tween_property(fade_overlay, "modulate:a", 1.0, 1.5) # Fade to black in 1.5s

	await get_tree().create_timer(1.5).timeout
	get_tree().change_scene_to_file(game_scene_path)
