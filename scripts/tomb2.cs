using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;

public partial class tomb2 : CharacterBody2D
{
	private Label dialogueText;
	private VBoxContainer choicesContainer;
	private Timer dialogueTimer;
	private Godot.Panel dialoguePanel;
	private CharacterController player;
	private AnimatedSprite2D graveSprite;
	private AnimatedSprite2D interactSprite;

	private int currentIndex = 0;
	private int hopeScore = 0;
	private bool playerNearby = false;
	private bool dialogueActive = false;

	private class DialogueEntry
	{
		public string Name;
		public string Text;
		public float Delay;
		public string Animation;
		public List<(string ChoiceText, int ScoreChange, string ResponseAnimation, int NextIndex)> Choices;
	}

	private List<DialogueEntry> dialogues = new List<DialogueEntry>
	{
		new DialogueEntry { 
			Name = "It Reads", 
			Text = "Here lies Reese, a beloved kitten.", 
			Delay = 2f, 
			Animation = "dirty", 
			Choices = new List<(string, int, string, int)>
			
			{
				("Pay Respects and clean grave", +2, "", 1),
				("A kitten (ignore)?", -1, "", 2),
			}
		},
		new DialogueEntry { Name = "Grave", Text = "You're a good person traveller safe travels ahead", Delay = 2f, Animation = "clean", Choices = new List<(string, int, string, int)>() },
		new DialogueEntry { Name = "Grave", Text = "No one ever cleans this grave", Delay = 2f, Animation = "dirty", Choices = new List<(string, int, string, int)>() }
	};

	public override void _Ready()
	{
		dialogueText = GetNodeOrNull<Label>("Control/Panel/Label");
		choicesContainer = GetNodeOrNull<VBoxContainer>("Control/Panel/VBoxContainer");
		dialogueTimer = GetNodeOrNull<Timer>("Timer");
		dialoguePanel = GetNodeOrNull<Godot.Panel>("Control/Panel");
		graveSprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
		interactSprite = GetNodeOrNull<AnimatedSprite2D>("interactable");

		player = GetTree().GetFirstNodeInGroup("player") as CharacterController;

		if (dialoguePanel != null) 
			dialoguePanel.Visible = false;
		if(interactSprite != null){
			interactSprite.Visible = true;
			interactSprite.Play("default");
		}
		if(graveSprite != null)
			graveSprite.Play("dirty");

		GetNode<Area2D>("Area2D").BodyEntered += OnPlayerEntered;
		GetNode<Area2D>("Area2D").BodyExited += OnPlayerExited;
	}

	private void OnPlayerEntered(Node body)
	{
		if (body.IsInGroup("player")) 
		{
			playerNearby = true;
		}
	}

	private void OnPlayerExited(Node body)
	{
		if (body is Player)
		{
			playerNearby = false;
			dialoguePanel.Visible = false;
			dialogueActive = false;
		}
	}

	public override void _Process(double delta)
	{
		if (player == null)
		{
			player = GetTree().GetFirstNodeInGroup("player") as CharacterController;
		}

		if (playerNearby && Input.IsActionJustPressed("interact") && !dialogueActive)
		{
			OpenDialogue();
		}
	}


	private void OpenDialogue()
	{
		dialoguePanel.Visible = true;
		dialogueActive = true;
		ShowNextDialogue();
		interactSprite.Visible = false;
	}

	private async void ShowNextDialogue()
	{
		if (currentIndex < dialogues.Count)
		{
			var entry = dialogues[currentIndex];
			dialogueText.Text = "";

			if (graveSprite != null)
			{
				graveSprite.Play(entry.Animation); 
			}

			choicesContainer.Hide();
			await TypeText($"{entry.Name}: {entry.Text}");

			if (entry.Choices.Count > 0)
			{
				DisplayChoices(entry.Choices);
			}
			else
			{
				await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
				dialoguePanel.Visible = false;
				dialogueActive = false;
			}
		}
	}

	private async Task TypeText(string fullText)
	{
		dialogueText.Text = "";
		foreach (char letter in fullText)
		{
			dialogueText.Text += letter;
			await ToSignal(GetTree().CreateTimer(0.025f), "timeout");
		}
	}

	private void DisplayChoices(List<(string ChoiceText, int ScoreChange, string ResponseAnimation, int NextIndex)> choices)
	{
		choicesContainer.Show();
		foreach (Node child in choicesContainer.GetChildren())
		{
			child.QueueFree();
		}

		foreach (var choice in choices)
		{
			Button choiceButton = new Button
			{
				Text = choice.ChoiceText,
				SizeFlagsHorizontal = Control.SizeFlags.Expand,
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin
			};
			choiceButton.Pressed += () => OnChoiceSelected(choice.ScoreChange, choice.ResponseAnimation, choice.NextIndex);
			choicesContainer.AddChild(choiceButton);
		}
	}
	private void OnChoiceSelected(int scoreChange, string responseAnimation, int nextIndex)
	{
		if (scoreChange > 0)
		{
			GameState.Instance.IncreaseHope(scoreChange);
		}
		else if (scoreChange < 0)
		{
			GameState.Instance.DecreaseHope(-scoreChange);
		}
		currentIndex = nextIndex;
		ShowNextDialogue();
	} 
}
