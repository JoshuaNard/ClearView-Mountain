extends Node2D

@onready var fade_overlay: ColorRect = $CanvasLayer/FadeOverlay

func _ready():
	if fade_overlay:
		fade_overlay.visible = true  # Ensure it's visible at the start
		fade_overlay.modulate = Color(0, 0, 0, 1)  # Start as fully black
		start_fade_in()

func start_fade_in():
	if fade_overlay:
		var tween = get_tree().create_tween()
		tween.tween_property(fade_overlay, "modulate:a", 0.0, 2.5)  # Fade from black in 2.5 seconds
		
		await tween.finished
		fade_overlay.visible = false
