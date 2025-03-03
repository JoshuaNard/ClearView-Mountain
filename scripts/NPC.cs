using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class NPC : CharacterBody2D
{
	private Label dialogueText;
	private VBoxContainer choicesContainer;
	private Timer dialogueTimer;
	private Godot.Panel dialoguePanel;
	private CharacterController player;
	private AnimatedSprite2D npcSprite;
	private AnimatedSprite2D interactSprite;
	private Sprite2D goldBootsSprite;
	private bool hasBeer = false;
	private bool hasGoldBoots = false;

	private int currentIndex = 0;
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
			Name = "Bob T", 
			Text = "Hey, traveler! I need a drink... got anything?", 
			Delay = 2f, 
			Animation = "idle", 
			Choices = new List<(string, int, string, int)>
			
			{
				("Give Beer", +2, "Happy", 1),
				("Steal Boots", -8, "Angry", 2),
			}
		},
		new DialogueEntry { Name = "Bob T", Text = "Thanks! Here, take these special boots I found! They made me feel much lighter!", Delay = 2f, Animation = "has beer", Choices = new List<(string, int, string, int)>() },
		new DialogueEntry { Name = "Bob T", Text = "Hey! You little thief!", Delay = 2f, Animation = "idle", Choices = new List<(string, int, string, int)>() }
	};

	public override void _Ready()
	{
		dialogueText = GetNodeOrNull<Label>("Control/Panel/Label");
		choicesContainer = GetNodeOrNull<VBoxContainer>("Control/Panel/VBoxContainer");
		dialogueTimer = GetNodeOrNull<Timer>("Timer");
		dialoguePanel = GetNodeOrNull<Godot.Panel>("Control/Panel");
		npcSprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
		interactSprite = GetNodeOrNull<AnimatedSprite2D>("interactable");
		goldBootsSprite = GetNodeOrNull<Sprite2D>("goldboots");

		player = GetTree().GetFirstNodeInGroup("player") as CharacterController;

		if (dialoguePanel != null) 
			dialoguePanel.Visible = false;
		if(interactSprite != null){
			interactSprite.Visible = true;
			interactSprite.Play("default");
		}

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

			if (npcSprite != null)
			{

				npcSprite.Play(entry.Animation);
			}
			

			choicesContainer.Hide();
			await TypeText($"{entry.Name}: {entry.Text}");

			if (entry.Choices.Count > 0)
			{
				DisplayChoices(entry.Choices);
			}
			else
			{
				await ToSignal(GetTree().CreateTimer(1.5f), "timeout");
				dialoguePanel.Visible = false;
				dialogueActive = false;
				hasBeer = true;
				if (goldBootsSprite != null)
					player.SetGoldBoots(true);
					goldBootsSprite.Visible = false; // Boots disappear when given
					
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
		if(scoreChange > 0){
				GameState.Instance.IncreaseHope(scoreChange);
			}
			else if(scoreChange < 0){
				GameState.Instance.DecreaseHope(-scoreChange);
			}
		currentIndex = nextIndex;

		 if (nextIndex == 2) // If player steals boots
		{
			hasGoldBoots = true;
			if (goldBootsSprite != null)
				player.SetGoldBoots(true);
				goldBootsSprite.Visible = false; // Hide boots immediately

		}

		ShowNextDialogue();
	} 
}
