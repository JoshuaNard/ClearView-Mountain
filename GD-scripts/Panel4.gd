extends Panel

var current_index = 0

@onready var dialogue_text = $Label
@onready var dialogue_timer = $Timer
@onready var fade_overlay = $"/root/decentEnding/CanvasLayer/FadeOverlay"

var dialogues = []

func _ready():
	# Hide the panel at start
	visible = false

	# Generate the final dialogue sequence
	generate_final_dialogue()

	# Start the final scene after a short delay
	get_tree().create_timer(9).timeout.connect(start_final_dialogue)

func generate_final_dialogue():
	dialogues.clear()

	dialogues.append({"name": "You", "text": "I almost lost you...", "delay": 2.0})
	dialogues.append({"name": "Wife", "text": "I know...", "delay": 2.0})
	dialogues.append({"name": "You", "text": "If I had been just a little slower—", "delay": 2.5})
	dialogues.append({"name": "Wife", "text": "But you were not. You were there when I needed you most.", "delay": 3.0})
	dialogues.append({"name": "Wife", "text": "The doctors said another few hours and it would've been too late.", "delay": 3.0})
	dialogues.append({"name": "You", "text": "I don not even want to think about that.", "delay": 2.0})
	dialogues.append({"name": "Wife", "text": "Me neither.", "delay": 2.0})
	dialogues.append({"name": "Wife", "text": "But… I am still here. And that is because of you.", "delay": 3.0})
	dialogues.append({"name": "You", "text": "Yeah... but I wish I had gotten to you sooner.", "delay": 2.5})
	dialogues.append({"name": "Wife", "text": "You did everything you could. And in the end... it was enough.", "delay": 3.0})
	dialogues.append({"name": "You", "text": "I just keep thinking... what if next time, I am too late?", "delay": 3.0})
	dialogues.append({"name": "Wife", "text": "Then we don’t let there be a next time.", "delay": 2.5})
	dialogues.append({"name": "Wife", "text": "From now on... we face everything together.", "delay": 3.0})
	dialogues.append({"name": "You", "text": "Yeah... together.", "delay": 3.0})

func start_final_dialogue():
	print("✅ Final dialogue started.")
	visible = true
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
