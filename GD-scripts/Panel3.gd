extends Panel

var current_index = 0

@onready var dialogue_text = $Label
@onready var dialogue_timer = $Timer
@onready var fade_overlay = get_node_or_null("/root/GoodEnding/CanvasLayer/FadeOverlay")  # FIX: Get node properly

var dialogues = []

func _ready():
	# Hide the panel at the start
	visible = false

	# Generate the final dialogue sequence
	generate_final_dialogue()

	# Start the final scene after a short delay
	get_tree().create_timer(9).timeout.connect(start_final_dialogue)

func generate_final_dialogue():
	dialogues.clear()

	# Player and wife reflecting on everything that happened
	dialogues.append({"name": "You", "text": "It still does not feel real…", "delay": 2.0})
	dialogues.append({"name": "Wife", "text": "I know. Everything happened so fast.", "delay": 2.0})
	dialogues.append({"name": "You", "text": "When they told me what was happening... I did not know if I would ever get to hear your voice again.", "delay": 3.0})
	dialogues.append({"name": "Wife", "text": "I did not know if I would either.", "delay": 2.0})
	dialogues.append({"name": "Wife", "text": "But you never gave up on me. Even when things looked impossible.", "delay": 3.0})
	dialogues.append({"name": "You", "text": "I could not. I would not.", "delay": 2.0})
	dialogues.append({"name": "Wife", "text": "It must have been so hard... going through all of that alone.", "delay": 3.0})
	dialogues.append({"name": "You", "text": "Yeah. There were moments when I thought I wouldn’t make it.", "delay": 2.0})
	dialogues.append({"name": "You", "text": "But every time I wanted to stop, I thought about you.", "delay": 3.0})
	dialogues.append({"name": "Wife", "text": "And now… here we are.", "delay": 2.0})
	dialogues.append({"name": "You", "text": "Here we are.", "delay": 2.0})
	dialogues.append({"name": "Wife", "text": "A second chance... together.", "delay": 3.0})
	dialogues.append({"name": "You", "text": "Yeah. Together.", "delay": 3.0})

func start_final_dialogue():
	visible = true # Show the UI panel
	show_next_dialogue()

func show_next_dialogue():
	if current_index < dialogues.size():
		var entry = dialogues[current_index]
		
		# Ensure text is cleared before typing effect
		dialogue_text.text = ""

		await type_text(entry["name"] + ": " + entry["text"])

		current_index += 1
		await get_tree().create_timer(entry["delay"]).timeout

		show_next_dialogue()
	else:
		await get_tree().create_timer(3.5).timeout
		end_game_sequence()

func type_text(full_text):
	dialogue_text.text = ""
	for letter in full_text:
		dialogue_text.text += letter
		await get_tree().create_timer(0.025).timeout

func end_game_sequence():
	visible = false
	await get_tree().create_timer(2).timeout

	# Handle screen fade-out
	if fade_overlay and is_instance_valid(fade_overlay):
		fade_overlay.visible = true
		var tween = get_tree().create_tween()
		tween.tween_property(fade_overlay, "modulate:a", 1.0, 5.0) # Fade to black over 5 seconds

		await get_tree().create_timer(7).timeout

	# FIX: Ensure scene actually exists before switching
	if ResourceLoader.exists("res://GameJam/EndScreen.tscn"):
		get_tree().change_scene_to_file("res://GameJam/EndScreen.tscn")
	else:
		push_error("Scene 'res://GameJam/EndScreen.tscn' not found!")
