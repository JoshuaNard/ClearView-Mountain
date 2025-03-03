extends Node

var hope_score: int = 8  # Default Hope Score

func _ready():
	print("GameState Loaded")  # Debugging check

func increase_hope(amount: int):
	hope_score += amount
	print("Hope Score Increased: ", hope_score)  # Debugging

func decrease_hope(amount: int):
	hope_score -= amount
	print("Hope Score Decreased: ", hope_score)  # Debugging
