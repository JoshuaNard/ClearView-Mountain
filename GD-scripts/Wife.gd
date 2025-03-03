extends Node2D

@onready var wife: AnimatedSprite2D = $AnimatedSprite2D

func _ready():
    wife.play("default")
