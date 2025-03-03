extends Node2D

@onready var torch: AnimatedSprite2D = $torchAnim

func _ready():
    torch.play("lit")
