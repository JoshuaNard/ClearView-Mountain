extends Area2D

@onready var fire_animation = $AnimatedSprite2D
@onready var campfire_light = $Light2D
@onready var embers = $Embers
@onready var smoke = $Smoke

var is_lit = false

func _ready():
    # Ensure nodes exist
    if not fire_animation or not campfire_light or not embers or not smoke:
        push_error("Campfire: Missing required nodes! Check scene hierarchy.")
        return

    # Default state: Unlit fire
    fire_animation.play("unlit fire")  # Play the unlit fire animation
    campfire_light.visible = false     # Light is OFF
    embers.emitting = false            # Embers OFF
    smoke.emitting = true              # Smoke ON

    connect("body_entered", _on_player_entered)

func _on_player_entered(body):
    if body.is_in_group("player"):
        if not body.has_method("set_respawn_point"):
            push_error("Campfire: Player does not have 'set_respawn_point' method!")
            return

        # Set player's respawn point
        body.set_respawn_point(global_position)

        # Light the campfire if it's not already lit
        if not is_lit:
            is_lit = true
            fire_animation.play("burn")   # Switch to fire animation
            campfire_light.visible = true # Show light
            embers.emitting = true        # Start embers
            smoke.emitting = false        # Stop smoke
            flicker_light()               # Make light flicker

func flicker_light():
    var tween = get_tree().create_tween().set_loops()
    tween.tween_property(campfire_light, "energy", 1.2, randf_range(0.1, 0.3))
    tween.tween_property(campfire_light, "energy", 2.0, randf_range(0.1, 0.3))
