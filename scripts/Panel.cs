using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;


public partial class Panel : Control
{
	private Label dialogueText;
	private VBoxContainer choicesContainer;
	private Timer dialogueTimer;
	private Player player;

	private int currentIndex = 0;

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
			Name = "Madison", 
			Text = "Hi, this is Madison from Clearview Valley Medical Center.", 
			Delay = 2f, 
			Animation = "Phone call Listen", 
			Choices = new List<(string, int, string, int)>
			{
				("Who are you?", 0, "Phone Call Talk", 1),
				("Is this about my wife?", +1, "Phone Call Talk", 2)
			}
		},

		new DialogueEntry { 
			Name = "Madison", 
			Text = "I'm Madison, your wife's nurse. I’ve been caring for her.", 
			Delay = 2f, 
			Animation = "Phone call Listen", 
			Choices = new List<(string, int, string, int)>
			{
				("How’s my wife doing?", 0, "Phone Call Talk", 2),
				("Do you need me to come in?", +1, "Phone Call Talk", 3)
			}
		},

	
		new DialogueEntry { 
			Name = "Madison", 
			Text = "I have some news… some good, some bad. Your wife has stage 4 brain cancer.", 
			Delay = 3f, 
			Animation = "Phone call Listen", 
			Choices = new List<(string, int, string, int)>
			{
				("What?! No way!", -2, "Phone call Listen", 4),
				("Please, tell me there's hope.", +2, "Phone Call Talk", 5)
			} 
		},

		
		new DialogueEntry { 
			Name = "Madison", 
			Text = "Not necessarily, but I need you to hear what I have to say first.", 
			Delay = 2.5f, 
			Animation = "Phone call Listen", 
			Choices = new List<(string, int, string, int)>
			{
				("Okay, I'm listening.", +1, "Phone Call Talk", 2),
				("Just tell me what I need to do.", 0, "Phone Call Talk", 2)
			}
		},

		
		new DialogueEntry { 
			Name = "Madison", 
			Text = "I know this is a lot to take in, but we do have options.", 
			Delay = 2.5f, 
			Animation = "Phone call Listen", 
			Choices = new List<(string, int, string, int)>
			{
				("Like what?", -1, "Phone Call Talk", 5),
				("I need to help her.", +2, "Phone Call Talk", 6)
			}
		},

		new DialogueEntry { 
			Name = "Madison", 
			Text = "We've been using experimental mushrooms to treat this. Nearly 100% success.", 
			Delay = 2.5f, 
			Animation = "Phone call Listen", 
			Choices = new List<(string, int, string, int)>
			{
				("Let's get her started on them!", +3, "Phone Call Talk", 6),
				("Why wasn’t she treated sooner?!", -3, "Phone call Listen", 6)
			} 
		},

		
		new DialogueEntry { 
			Name = "Madison", 
			Text = "We need more mushrooms, but Dr. Caleb who gathers them is on leave. He says it's an easy hike, one he takes for fun on the weekends.", 
			Delay = 2f, 
			Animation = "Phone call Listen", 
			Choices = new List<(string, int, string, int)>
			{
				("I’ll get them myself!", +3, "Phone Call Talk", 8),
				("Of course. Just my luck…", -2, "Phone call Listen", 9)
			} 
		},

		
		new DialogueEntry { 
			Name = "Madison", 
			Text = "Thank you… but listen carefully. You need to leave **now**.", 
			Delay = 2.5f, 
			Animation = "Phone call Listen", 
			Choices = new List<(string, int, string, int)>
			{
				("Why the rush?", 0, "Phone Call Talk", 10),
				("I'm already on my way.", +2, "Phone Call Talk", 11)
			}
		},

		
		new DialogueEntry { 
			Name = "Madison", 
			Text = "By morning, they’ll be gone. **This is our last chance.**", 
			Delay = 3f, 
			Animation = "Phone call Listen", 
			Choices = new List<(string, int, string, int)>
			{
				("I'm going. I won’t let her die", +3, "Phone Call Talk", 11),
				("There has to be another way…", -2, "Phone Call Talk", 12)
			} 
		},

	
		new DialogueEntry { 
			Name = "Madison", 
			Text = "**There isn’t.** If you don’t go now, we lose her forever.", 
			Delay = 2.5f, 
			Animation = "Phone call Listen", 
			Choices = new List<(string, int, string, int)>
			{
				("Fine. I’m going.", 0, "Phone Call Talk", 11),
				("Damn it… I have no choice.", -1, "Phone Call Talk", 11)
			}
		},

		
			new DialogueEntry { 
			Name = "Madison", 
			Text = "Be careful. You don’t have much time. **Find them before it’s too late.**", 
			Delay = 3f, 
			Animation = "Phone call Listen", 
			Choices = new List<(string, int, string, int)>()
		}
};




	public override void _Ready()
	{
		// Get UI elements
		dialogueText = GetNodeOrNull<Label>("Label");
		choicesContainer = GetNodeOrNull<VBoxContainer>("ChoicesContainer");
		dialogueTimer = GetNodeOrNull<Timer>("Timer");
		player = GetNodeOrNull<Player>("../player"); 

		// Get the Panel itself
		Panel panel = GetNodeOrNull<Panel>(".");

		// Hide the Panel at the start
		if (panel != null)
		{
			panel.Visible = false;
		}

		// Debugging: Print missing nodes
		if (dialogueText == null) GD.PrintErr("Error: Label (dialogueText) not found!");
		if (choicesContainer == null) GD.PrintErr("Error: VBoxContainer (choicesContainer) not found!");
		if (dialogueTimer == null) GD.PrintErr("Error: Timer (dialogueTimer) not found!");
		if (player == null) GD.PrintErr("Error: Player not found!");

		// Wait 15 seconds before starting the dialogue
		GetTree().CreateTimer(13.5).Timeout += () => 
		{
			ShowPanel(); // Show panel when dialogue starts
			ShowNextDialogue();
		};
	}
	private async Task TypeText(string fullText)
	{
		dialogueText.Text = ""; // Clear previous text

		foreach (char letter in fullText)
		{
			dialogueText.Text += letter;
			await ToSignal(GetTree().CreateTimer(0.025f), "timeout"); // Simulates typing speed
		}
	}







		private async void ShowNextDialogue()
		{
			if (currentIndex < dialogues.Count)
			{
				var entry = dialogues[currentIndex];
				dialogueText.Text = ""; // Clear text

				player.PlayAnimation(entry.Animation);
				choicesContainer.Hide();

				await TypeText($"{entry.Name}: {entry.Text}"); // Use Typewriter Effect

				if (entry.Choices.Count > 0)
				{
					DisplayChoices(entry.Choices);
				}
				else
				{
					await ToSignal(GetTree().CreateTimer(2.5f), "timeout"); 
					await EndCutsceneSequence();
				}
			}
		}






	private void DisplayText(string fullText)
	{
		dialogueText.Text += fullText;
	}

	private void DisplayChoices(List<(string ChoiceText, int ScoreChange, string ResponseAnimation, int NextIndex)> choices)
	{
		choicesContainer.Show();

		// Remove old buttons
		foreach (Node child in choicesContainer.GetChildren())
		{
			child.QueueFree();
		}

		foreach (var choice in choices)
		{
			string shortenedText = choice.ChoiceText.Length > 50 ? choice.ChoiceText.Substring(0, 47) + "..." : choice.ChoiceText;

			Button choiceButton = new Button
			{
				Text = shortenedText,
				SizeFlagsHorizontal = Control.SizeFlags.Expand,
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin
			};

			choiceButton.Pressed += () => OnChoiceSelected(choice.ScoreChange, choice.ResponseAnimation, choice.NextIndex);
			choicesContainer.AddChild(choiceButton);
		}
	}




	private async void OnChoiceSelected(int scoreChange, string responseAnimation, int nextIndex)
{

	// Update the global hopeScore
	if (scoreChange > 0)
	{
		GameState.Instance.IncreaseHope(scoreChange);
	}
	else if (scoreChange < 0)
	{
		GameState.Instance.DecreaseHope(-scoreChange);
	}

	// Play the animation
	player.PlayAnimation(responseAnimation);


	// Remove choice buttons
	foreach (Node child in choicesContainer.GetChildren())
	{
		child.QueueFree();
	}

	// Move to next dialogue entry
	currentIndex = nextIndex;

	if (currentIndex >= dialogues.Count) // If it's the last dialogue
	{
		dialogueText.Text = "Be careful. You don’t have much time. **Find them before it’s too late.**";
		await TypeText(dialogueText.Text);
		await ToSignal(GetTree().CreateTimer(2.5f), "timeout"); 
		await EndCutsceneSequence(); // Hide panel and transition
	}
	else
	{
		ShowNextDialogue();
	}
}




	private void ContinueDialogue()
	{
		currentIndex++;
		ShowNextDialogue();
	}

  private async Task EndCutsceneSequence()
		{

			// Hide the dialogue panel
			this.Visible = false;

			// Get the Camera2D
			Camera2D camera = GetNodeOrNull<Camera2D>("../player/Camera2D");

			if (camera != null)
			{
				camera.Reparent(GetParent()); // Detach camera from the player
			}

			// Get the fade overlay
			ColorRect fadeOverlay = GetNodeOrNull<ColorRect>("../CanvasLayer/FadeOverlay");

			if (fadeOverlay != null)
			{
				fadeOverlay.Visible = true; // Make it visible before fading
				Tween tween = GetTree().CreateTween();
				tween.TweenProperty(fadeOverlay, "modulate:a", 1.0f, 7.0f); // Fade to black in 2s
				// Start walking animation
				player.PlayAnimation("Walk");

				// Move player off-screen
				float moveTime = 8f; 
				float moveSpeed = 50f;
				float elapsed = 0f;

				while (elapsed < moveTime)
				{
					player.Position += new Vector2(moveSpeed * (float)GetProcessDeltaTime(), 0);
					elapsed += (float)GetProcessDeltaTime();
					await ToSignal(GetTree(), "process_frame");

				}
				GetTree().ChangeSceneToFile("res://GameJam/world.tscn");
			}

			

			// Switch scene after exiting
			
		}




	private void ShowPanel()
	{
		this.Visible = true; // Make Panel UI visible
	}



}
