extends Control

var current_index = 0

@onready var dialogue_text = $Label
@onready var choices_container = $ChoicesContainer
@onready var dialogue_timer = $Timer
@onready var player = $"../player"
@onready var fade_overlay = $"../CanvasLayer/FadeOverlay"

var dialogues = [
	{
		"name": "Madison",
		"text": "Hi, this is Madison from Clearview Valley Medical Center.",
		"delay": 2.0,
		"animation": "Phone call Listen",
		"choices": [
			{ "text": "Who are you?", "score_change": 0, "response_animation": "Phone Call Talk", "next_index": 1 },
			{ "text": "Is this about my wife?", "score_change": 1, "response_animation": "Phone Call Talk", "next_index": 2 }
		]
	},
	{
		"name": "Madison",
		"text": "I am Madison, your wifes nurse. I have been caring for her.",
		"delay": 2.0,
		"animation": "Phone call Listen",
		"choices": [
			{ "text": "How is my wife doing?", "score_change": 0, "response_animation": "Phone Call Talk", "next_index": 2 },
			{ "text": "Do you need me to come in?", "score_change": 1, "response_animation": "Phone Call Talk", "next_index": 3 }
		]
	},
	{
		"name": "Madison",
		"text": "I have some news… some good, some bad. Your wife has stage 4 brain cancer.",
		"delay": 3.0,
		"animation": "Phone call Listen",
		"choices": [
			{ "text": "What?! No way!", "score_change": -2, "response_animation": "Phone call Listen", "next_index": 4 },
			{ "text": "Please, tell me there is hope.", "score_change": 2, "response_animation": "Phone Call Talk", "next_index": 5 }
		]
	},
	{
		"name": "Madison",
		"text": "Not necessarily, but I need you to hear what I have to say first.",
		"delay": 2.5,
		"animation": "Phone call Listen",
		"choices": [
			{ "text": "Okay, I am listening.", "score_change": 1, "response_animation": "Phone Call Talk", "next_index": 2 },
			{ "text": "Just tell me what I need to do.", "score_change": 0, "response_animation": "Phone Call Talk", "next_index": 2 }
		]
	},
	{
		"name": "Madison",
		"text": "I know this is a lot to take in, but we do have options.",
		"delay": 2.5,
		"animation": "Phone call Listen",
		"choices": [
			{ "text": "Like what?", "score_change": -1, "response_animation": "Phone Call Talk", "next_index": 5 },
			{ "text": "I need to help her.", "score_change": 2, "response_animation": "Phone Call Talk", "next_index": 6 }
		]
	},
	{
		"name": "Madison",
		"text": "We have been using experimental mushrooms to treat this. Nearly 100% success.",
		"delay": 2.5,
		"animation": "Phone call Listen",
		"choices": [
			{ "text": "Lets get her started on them!", "score_change": 3, "response_animation": "Phone Call Talk", "next_index": 6 },
			{ "text": "Why was she not treated sooner?!", "score_change": -3, "response_animation": "Phone call Listen", "next_index": 6 }
		]
	},
	{
		"name": "Madison",
		"text": "We need more mushrooms, but Dr. Caleb who gathers them is on leave. He says it is an easy hike, one he takes for fun on the weekends.",
		"delay": 2.0,
		"animation": "Phone call Listen",
		"choices": [
			{ "text": "I will get them myself!", "score_change": 3, "response_animation": "Phone Call Talk", "next_index": 8 },
			{ "text": "Of course. Just my luck…", "score_change": -2, "response_animation": "Phone call Listen", "next_index": 9 }
		]
	},
	{
		"name": "Madison",
		"text": "Thank you… but listen carefully. You need to leave **now**.",
		"delay": 2.5,
		"animation": "Phone call Listen",
		"choices": [
			{ "text": "Why the rush?", "score_change": 0, "response_animation": "Phone Call Talk", "next_index": 10 },
			{ "text": "I am already on my way.", "score_change": 2, "response_animation": "Phone Call Talk", "next_index": 11 }
		]
	},
	{
		"name": "Madison",
		"text": "By morning, they will be gone. **This is our last chance.**",
		"delay": 3.0,
		"animation": "Phone call Listen",
		"choices": [
			{ "text": "I am going. I won’t let her die.", "score_change": 3, "response_animation": "Phone Call Talk", "next_index": 11 },
			{ "text": "There has to be another way…", "score_change": -2, "response_animation": "Phone Call Talk", "next_index": 12 }
		]
	},
	{
		"name": "Madison",
		"text": "**There is not.** If you do not go now, we lose her forever.",
		"delay": 2.5,
		"animation": "Phone call Listen",
		"choices": [
			{ "text": "Fine. I am going.", "score_change": 0, "response_animation": "Phone Call Talk", "next_index": 11 },
			{ "text": "Damn it… I have no choice.", "score_change": -1, "response_animation": "Phone Call Talk", "next_index": 11 }
		]
	},
	{
		"name": "Madison",
		"text": "Be careful. You do not have much time. **Find them before it is too late.**",
		"delay": 3.0,
		"animation": "Phone call Listen",
		"choices": []
	}
]

func _ready():
	visible = false

	if not dialogue_text:
		push_error("Error: Label (dialogue_text) not found!")
	if not choices_container:
		push_error("Error: VBoxContainer (choices_container) not found!")
	if not dialogue_timer:
		push_error("Error: Timer (dialogue_timer) not found!")
	if not player:
		push_error("Error: Player not found!")

	get_tree().create_timer(13.5).timeout.connect(_show_panel_and_start_dialogue)

func _show_panel_and_start_dialogue():
	show_panel()
	show_next_dialogue()

func show_panel():
	visible = true

func show_next_dialogue():
	if current_index < dialogues.size():
		var entry = dialogues[current_index]
		dialogue_text.text = ""

		if player and player.has_method("play_animation"):
			player.play_animation(entry["animation"])

		choices_container.hide()
		await type_text(entry["name"] + ": " + entry["text"])

		if entry["choices"].size() > 0:
			display_choices(entry["choices"])
		else:
			await get_tree().create_timer(2.5).timeout
			await end_cutscene_sequence()

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

	if player and player.has_method("play_animation"):
		player.play_animation(response_animation)

	for child in choices_container.get_children():
		child.queue_free()

	current_index = next_index

	if current_index < dialogues.size():
		show_next_dialogue()
	else:
		await type_text("Be careful. You don’t have much time. **Find them before it’s too late.**")
		await get_tree().create_timer(2.5).timeout
		await end_cutscene_sequence()

func end_cutscene_sequence():
	visible = false

	# Ensure the player is set before playing animation
	if player and player.has_method("play_animation"):
		player.play_animation("Walk")  # 🔥 **Fixed: Play walking animation after phone call!**

	# Detach the camera from the player
	var camera = get_node_or_null("../player/Camera2D")
	if camera:
		camera.reparent(get_parent())

	# Fade to black
	if fade_overlay:
		fade_overlay.visible = true
		var tween = get_tree().create_tween()
		tween.tween_property(fade_overlay, "modulate:a", 1.0, 7.0)

	# Start walking animation and move player off-screen
	var move_time = 8.0
	var move_speed = 50.0
	var elapsed = 0.0

	while elapsed < move_time:
		if player:
			player.position += Vector2(move_speed * get_process_delta_time(), 0)
		elapsed += get_process_delta_time()
		await get_tree().process_frame

	get_tree().change_scene_to_file("res://GameJam/world.tscn")  # 🔥 **Fixed: Ensure scene changes after cutscene**
