using Godot;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

public partial class Panel2 : Control
{
    private Label dialogueText;
    private Timer dialogueTimer;
    private Player2 player;
    private int currentIndex = 0;
    private int hopeScore = 0; // Retrieved from GameState

    private class DialogueEntry
    {
        public string Name;
        public string Text;
        public float Delay;
        public string Animation;
    }

    private List<DialogueEntry> dialogues = new List<DialogueEntry>();

    public override void _Ready()
    {
        // Get UI elements
        dialogueText = GetNodeOrNull<Label>("Label");
        dialogueTimer = GetNodeOrNull<Timer>("Timer");
        player = GetNodeOrNull<Player2>("../player2");

        // Hide panel at start
        this.Visible = false;

        // Retrieve the player's hope score
        if (GameState.Instance != null)
        {
            hopeScore = GameState.Instance.HopeScore;
        }

        // Generate the correct dialogue sequence based on Hope Score
        GenerateFinalDialogue();

        // Start the final scene after a short delay
        GetTree().CreateTimer(25).Timeout += () => StartFinalDialogue();
    }

    private void GenerateFinalDialogue()
    {
        dialogues.Clear();

        // Player initiates the call
        dialogues.Add(new DialogueEntry { Name = "You", Text = "Hello, is this Madison? I have the mushrooms.", Delay = 2f, Animation = "phone call" });

        if (hopeScore < 10)
        {
            // **Worst Ending: Wife Passed Away**
            dialogues.Add(new DialogueEntry { Name = "Madison", Text = "... I'm so sorry. We did everything we could, but we lost her an hour ago.", Delay = 3f, Animation = "phone call" });
            dialogues.Add(new DialogueEntry { Name = "Madison", Text = "If only we'd had the treatment sooner...", Delay = 2f, Animation = "phone call" });
            dialogues.Add(new DialogueEntry { Name = "You", Text = "No... No, no, no.", Delay = 3f, Animation = "phone call" });
        }
        else if (hopeScore >=10 && hopeScore <= 20)
        {
            // **Bittersweet Ending: Wife Survives, but is Weak**
            dialogues.Add(new DialogueEntry { Name = "Madison", Text = "You got them! Thank God! She's stable, but very weak.", Delay = 3f, Animation = "phone call" });
            dialogues.Add(new DialogueEntry { Name = "Madison", Text = "This will save her life, but recovery will be slow.", Delay = 3f, Animation = "phone call" });
            dialogues.Add(new DialogueEntry { Name = "You", Text = "I understand. Just... keep her safe.", Delay = 2f, Animation = "phone call" });
            dialogues.Add(new DialogueEntry { Name = "Madison", Text = "We will. Thank you for everything.", Delay = 2f, Animation = "phone call" });
        }
        else
        {
            // **Best Ending: Wife Makes a Full Recovery**
            dialogues.Add(new DialogueEntry { Name = "Madison", Text = "Oh, you're a hero! The hospital found another batch just in time.", Delay = 3f, Animation = "phone call" });
            dialogues.Add(new DialogueEntry { Name = "Madison", Text = "She's doing amazingly well! She might be discharged next week!", Delay = 3f, Animation = "phone call" });
            dialogues.Add(new DialogueEntry { Name = "You", Text = "I... I can't believe it. Thank you!", Delay = 2f, Animation = "phone call" });
            dialogues.Add(new DialogueEntry { Name = "Madison", Text = "No, thank you. She wouldn't be here without you.", Delay = 3f, Animation = "phone call 	" });
        }
    }

    private void StartFinalDialogue()
    {
        this.Visible = true; // Show the UI panel
        ShowNextDialogue();
    }

    private async void ShowNextDialogue()
    {
        if (currentIndex < dialogues.Count)
        {
            var entry = dialogues[currentIndex];
            dialogueText.Text = ""; // Clear text

            player.PlayAnimation(entry.Animation);
            await TypeText($"{entry.Name}: {entry.Text}");

            currentIndex++;
            await ToSignal(GetTree().CreateTimer(entry.Delay), "timeout");

            ShowNextDialogue();
        }
        else
        {
            await ToSignal(GetTree().CreateTimer(3.5f), "timeout");
			
            EndGameSequence();
        }
    }

    private async Task TypeText(string fullText)
    {
        dialogueText.Text = ""; // Clear previous text
        foreach (char letter in fullText)
        {
            dialogueText.Text += letter;
            await ToSignal(GetTree().CreateTimer(0.025f), "timeout"); // Typing effect
        }
    }

    private async void EndGameSequence()
    {
		this.Visible = false;
		player.PlayAnimation("idle");
		await ToSignal(GetTree().CreateTimer(2), "timeout");
        // Get the fade overlay
        ColorRect fadeOverlay = GetNodeOrNull<ColorRect>("../CanvasLayer/FadeOverlay");

        if (fadeOverlay != null)
        {
            fadeOverlay.Visible = true;
            Tween tween = GetTree().CreateTween();
            tween.TweenProperty(fadeOverlay, "modulate:a", 1.0f, 5.0f); // Fade to black

            await ToSignal(GetTree().CreateTimer(7), "timeout");
			if(hopeScore < 10){
				GetTree().ChangeSceneToFile("res://GameJam/EndScreen.tscn");
			}
             if (hopeScore >= 10 && hopeScore < 20)
                GetTree().ChangeSceneToFile("res://GameJam/DecentEnding.tscn");
            else if(hopeScore >= 20)
                GetTree().ChangeSceneToFile("res://GameJam/GoodEnding.tscn");
        }
    }
}
