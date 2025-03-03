extends ColorRect

@onready var black_screen = $BlackScreen
@onready var thank_you_label = $ThankYouLabel

func _ready():
    # Start fully transparent
    black_screen.modulate = Color(0, 0, 0, 0) # Invisible
    thank_you_label.modulate = Color(1, 1, 1, 0) # Invisible

    # Start fade-in
    fade_in()

func fade_in():
    var tween = get_tree().create_tween()
    tween.tween_property(black_screen, "modulate:a", 1.0, 3.0) # Fade in black screen
    await get_tree().create_timer(3.0).timeout # Wait for fade

    var text_tween = get_tree().create_tween()
    text_tween.tween_property(thank_you_label, "modulate:a", 1.0, 2.5) # Fade in text

    await get_tree().create_timer(5.0).timeout # Keep screen for 5 sec
    get_tree().quit() # Exit the game
