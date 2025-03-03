extends Control

var current_index = 0
var hope_score = 0 # Retrieved from GameState

@onready var dialogue_text = $Label
@onready var dialogue_timer = $Timer
@onready var player = $"../player2"
@onready var fade_overlay = $"../CanvasLayer/FadeOverlay"

var dialogues = []

func _ready():
	# Hide the panel at start
	visible = false

	hope_score = GameState.hope_score

	# Generate the correct dialogue sequence based on Hope Score
	generate_final_dialogue()

	# Start the final scene after a short delay
	get_tree().create_timer(25).timeout.connect(start_final_dialogue)

func generate_final_dialogue():
	dialogues.clear()

	# Player initiates the call
	dialogues.append({"name": "You", "text": "Hello, is this Madison? I have the mushrooms.", "delay": 2.0, "animation": "phone call"})

	if hope_score < 10:
		# **Worst Ending: Wife Passed Away**
		dialogues.append({"name": "Madison", "text": "... I am so sorry. We did everything we could, but we lost her an hour ago.", "delay": 3.0, "animation": "phone call"})
		dialogues.append({"name": "Madison", "text": "If only we had the treatment sooner...", "delay": 2.0, "animation": "phone call"})
		dialogues.append({"name": "You", "text": "No... No, no, no.", "delay": 3.0, "animation": "phone call"})
	elif hope_score >= 10 and hope_score <= 20:
		# **Bittersweet Ending: Wife Survives, but is Weak**
		dialogues.append({"name": "Madison", "text": "You got them! Thank God! She is stable, but very weak.", "delay": 3.0, "animation": "phone call"})
		dialogues.append({"name": "Madison", "text": "This will save her life, but recovery will be slow.", "delay": 3.0, "animation": "phone call"})
		dialogues.append({"name": "You", "text": "I understand. Just... keep her safe.", "delay": 2.0, "animation": "phone call"})
		dialogues.append({"name": "Madison", "text": "We will. Thank you for everything.", "delay": 2.0, "animation": "phone call"})
	else:
		# **Best Ending: Wife Makes a Full Recovery**
		dialogues.append({"name": "Madison", "text": "Oh, you are a hero! The hospital found another batch just in time.", "delay": 3.0, "animation": "phone call"})
		dialogues.append({"name": "Madison", "text": "She is doing amazingly well! She might be discharged next week!", "delay": 3.0, "animation": "phone call"})
		dialogues.append({"name": "You", "text": "I... I can't believe it. Thank you!", "delay": 2.0, "animation": "phone call"})
		dialogues.append({"name": "Madison", "text": "No, thank you. She wouldn't be here without you.", "delay": 3.0, "animation": "phone call"})

func start_final_dialogue():
	visible = true # Show the UI panel
	show_next_dialogue()

func show_next_dialogue():
	if current_index < dialogues.size():
		var entry = dialogues[current_index]
		dialogue_text.text = ""

		# Ensure player exists before playing animation
		if player and player.has_method("play_animation"):
			player.play_animation(entry["animation"])

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

	if player and player.has_method("play_animation"):
		player.play_animation("idle")

	await get_tree().create_timer(2).timeout

	if fade_overlay:
		fade_overlay.visible = true
		var tween = get_tree().create_tween()
		tween.tween_property(fade_overlay, "modulate:a", 1.0, 5.0) # Fade to black

		await get_tree().create_timer(7).timeout

		if hope_score < 10:
			get_tree().change_scene_to_file("res://GameJam/EndScreen.tscn")
		elif hope_score >= 10 and hope_score < 20:
			get_tree().change_scene_to_file("res://GameJam/DecentEnding.tscn")
		else:
			get_tree().change_scene_to_file("res://GameJam/GoodEnding.tscn")
