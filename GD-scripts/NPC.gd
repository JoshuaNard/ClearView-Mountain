extends CharacterBody2D

var player_nearby = false
var dialogue_active = false
var has_beer = false
var has_gold_boots = false
var current_index = 0

@onready var dialogue_text = $Control/Panel/Label
@onready var choices_container = $Control/Panel/VBoxContainer
@onready var dialogue_panel = $Control/Panel
@onready var npc_sprite = $AnimatedSprite2D
@onready var interact_sprite = $interactable
@onready var gold_boots_sprite = $goldboots
@onready var area_2d = $Area2D

var player

class DialogueEntry:
	var name = ""
	var text = ""
	var delay = 0.0
	var animation = ""
	var choices = []

var dialogues = [
	DialogueEntry.new(),
	DialogueEntry.new(),
	DialogueEntry.new()
]

func _ready():
	# Initialize dialogues
	dialogues[0].name = "Bob T"
	dialogues[0].text = "Hey, traveler! I need a drink... got anything?"
	dialogues[0].delay = 2.0
	dialogues[0].animation = "idle"
	dialogues[0].choices = [
		["Give Beer", 2, "Happy", 1],
		["Steal Boots", -8, "Angry", 2]
	]
	
	dialogues[1].name = "Bob T"
	dialogues[1].text = "Thanks! Here, take these special boots I found! They made me feel much lighter!"
	dialogues[1].delay = 2.0
	dialogues[1].animation = "has beer"
	dialogues[1].choices = []

	dialogues[2].name = "Bob T"
	dialogues[2].text = "Hey! You little thief!"
	dialogues[2].delay = 2.0
	dialogues[2].animation = "idle"
	dialogues[2].choices = []

	player = get_tree().get_first_node_in_group("player")

	if dialogue_panel:
		dialogue_panel.visible = false
	if interact_sprite:
		interact_sprite.visible = true
		interact_sprite.play("default")

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

		if npc_sprite:
			npc_sprite.play(entry.animation)

		choices_container.hide()
		await type_text(entry.name + ": " + entry.text)

		if entry.choices.size() > 0:
			display_choices(entry.choices)
		else:
			await get_tree().create_timer(1.5).timeout
			dialogue_panel.visible = false
			dialogue_active = false
			has_beer = true
			if gold_boots_sprite and player:
				player.set_gold_boots(true)
				gold_boots_sprite.visible = false  # Boots disappear when given

func type_text(full_text):
	dialogue_text.text = ""
	for letter in full_text:
		dialogue_text.text += letter
		await get_tree().create_timer(0.025).timeout

func display_choices(choices):
	choices_container.show()

	for child in choices_container.get_children():
		child.queue_free()

	for choice in choices:
		var choice_button = Button.new()
		choice_button.text = choice[0]
		choice_button.size_flags_horizontal = Control.SIZE_EXPAND
		choice_button.size_flags_vertical = Control.SIZE_SHRINK_BEGIN
		choice_button.connect("pressed", Callable(self, "_on_choice_selected").bind(choice[1], choice[2], choice[3]))
		choices_container.add_child(choice_button)

func _on_choice_selected(score_change, response_animation, next_index):
	if score_change > 0:
		GameState.increase_hope(score_change)
	elif score_change < 0:
		GameState.decrease_hope(abs(score_change))

	current_index = next_index

	if next_index == 2:  # If player steals boots
		has_gold_boots = true
		if gold_boots_sprite and player:
			player.set_gold_boots(true)
			gold_boots_sprite.visible = false  # Hide boots immediately

	show_next_dialogue()
