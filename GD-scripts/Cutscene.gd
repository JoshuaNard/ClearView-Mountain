extends Node2D

@onready var fade_overlay = $CanvasLayer/FadeOverlay

func _ready():
    # Ensure fade overlay starts fully black
    if fade_overlay:
        fade_overlay.modulate = Color(0, 0, 0, 1)
        start_fade_in()

func start_fade_in():
    if fade_overlay:
        var tween = get_tree().create_tween()
        tween.tween_property(fade_overlay, "modulate:a", 0.0, 1.5) # Fade from black in 1.5 seconds
        await tween.finished

        # Optional: Remove fade overlay after fade-in is complete
        fade_overlay.queue_free()
