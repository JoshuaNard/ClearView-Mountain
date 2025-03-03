extends CharacterBody2D

var player_nearby = false
var dialogue_active = false
var current_index = 0

@onready var dialogue_text = $Control/Panel/Label
@onready var choices_container = $Control/Panel/VBoxContainer
@onready var dialogue_panel = $Control/Panel
@onready var grave_sprite = $AnimatedSprite2D
@onready var interact_sprite = $interactable
@onready var area_2d = $Area2D

var player

var dialogues = [
	{
		"name": "It Reads",
		"text": "Here lies Paulie Barker, a local drunkard who wandered too far from the bar.",
		"delay": 2.0,
		"animation": "dirty",
		"choices": [
			{ "text": "Pay Respects and clean grave", "score_change": 2, "response_animation": "", "next_index": 1 },
			{ "text": "Ignore and move on", "score_change": 0, "response_animation": "", "next_index": 2 }
		]
	},
	{
		"name": "Grave",
		"text": "You're a good person traveler, safe travels ahead.",
		"delay": 2.0,
		"animation": "clean",
		"choices": []
	},
	{
		"name": "Grave",
		"text": "No one ever cleans this grave...",
		"delay": 2.0,
		"animation": "dirty",
		"choices": []
	}
]

func _ready():
	player = get_tree().get_first_node_in_group("player")

	if dialogue_panel:
		dialogue_panel.visible = false
	if interact_sprite:
		interact_sprite.visible = true
		interact_sprite.play("default")
	if grave_sprite:
		grave_sprite.play("dirty")

	area_2d.connect("body_entered", _on_player_entered)
	area_2d.connect("body_exited", _on_player_exited)

func _process(delta):
	if not player:
		player = get_tree().get_first_node_in_group("player")

	if player_nearby and Input.is_action_just_pressed("interact") and not dialogue_active:
		open_dialogue()

func _on_player_entered(body):
	if body.is_in_group("player"):
		player_nearby = true

func _on_player_exited(body):
	if body.is_in_group("player"):
		player_nearby = false
		dialogue_panel.visible = false
		dialogue_active = false

func open_dialogue():
	dialogue_panel.visible = true
	dialogue_active = true
	show_next_dialogue()
	interact_sprite.visible = false

func show_next_dialogue():
	if current_index < dialogues.size():
		var entry = dialogues[current_index]
		dialogue_text.text = ""

		if grave_sprite:
			grave_sprite.play(entry["animation"])

		choices_container.hide()
		await type_text(entry["name"] + ": " + entry["text"])

		if entry["choices"].size() > 0:
			display_choices(entry["choices"])
		else:
			await get_tree().create_timer(0.5).timeout
			dialogue_panel.visible = false
			dialogue_active = false

func type_text(full_text):
	dialogue_text.text = ""
	for letter in full_text:
		dialogue_text.text += letter
		await get_tree().create_timer(0.025).timeout

func display_choices(choices):
	choices_container.show()

	# Remove old buttons
	for child in choices_container.get_children():
		child.queue_free()

	for choice in choices:
		var choice_button = Button.new()
		choice_button.text = choice["text"]
		choice_button.size_flags_horizontal = Control.SIZE_EXPAND
		choice_button.size_flags_vertical = Control.SIZE_SHRINK_BEGIN
		choice_button.connect("pressed", Callable(self, "_on_choice_selected").bind(choice["score_change"], choice["response_animation"], choice["next_index"]))
		choices_container.add_child(choice_button)

func _on_choice_selected(score_change, response_animation, next_index):
	if score_change > 0:
		GameState.increase_hope(score_change)
	elif score_change < 0:
		GameState.decrease_hope(abs(score_change)) 

	current_index = next_index
	show_next_dialogue()
