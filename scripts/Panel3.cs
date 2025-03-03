using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Panel3 : Panel
{
    private Label dialogueText;
    private Timer dialogueTimer;
    private ColorRect fadeOverlay;
    private int currentIndex = 0;

    private class DialogueEntry
    {
        public string Name;
        public string Text;
        public float Delay;
    }

    private List<DialogueEntry> dialogues = new List<DialogueEntry>();

    public override void _Ready()
    {
        // Get UI elements inside the Panel
        dialogueText = GetNodeOrNull<Label>("Label");
        dialogueTimer = GetNodeOrNull<Timer>("Timer");

        // Find the fade overlay inside the CanvasLayer
        fadeOverlay = GetNodeOrNull<ColorRect>("/root/GoodEnding/CanvasLayer/FadeOverlay");


        // Hide panel at start
        this.Visible = false;

        // Generate the final dialogue sequence
        GenerateFinalDialogue();

        // Start the final scene after a short delay
        GetTree().CreateTimer(9).Timeout += StartFinalDialogue;
    }

   private void GenerateFinalDialogue()
    {
        dialogues.Clear();

        // Player and wife are outside, reflecting on everything that happened
        dialogues.Add(new DialogueEntry { Name = "You", Text = "It still doesn’t feel real…", Delay = 2f });
        
        dialogues.Add(new DialogueEntry { Name = "Wife", Text = "I know. Everything happened so fast.", Delay = 2f });
        
        dialogues.Add(new DialogueEntry { Name = "You", Text = "When they told me what was happening... I didn’t know if I’d ever get to hear your voice again.", Delay = 3f });

        dialogues.Add(new DialogueEntry { Name = "Wife", Text = "I didn’t know if I would either.", Delay = 2f });

        dialogues.Add(new DialogueEntry { Name = "Wife", Text = "But you never gave up on me. Even when things looked impossible.", Delay = 3f });

        dialogues.Add(new DialogueEntry { Name = "You", Text = "I couldn’t. I wouldn’t.", Delay = 2f });

        dialogues.Add(new DialogueEntry { Name = "Wife", Text = "It must have been so hard... going through all of that alone.", Delay = 3f });

        dialogues.Add(new DialogueEntry { Name = "You", Text = "Yeah. There were moments when I thought I wouldn’t make it.", Delay = 2f });

        dialogues.Add(new DialogueEntry { Name = "You", Text = "But every time I wanted to stop, I thought about you.", Delay = 3f });

        dialogues.Add(new DialogueEntry { Name = "Wife", Text = "And now… here we are.", Delay = 2f });

        dialogues.Add(new DialogueEntry { Name = "You", Text = "Here we are.", Delay = 2f });

        dialogues.Add(new DialogueEntry { Name = "Wife", Text = "A second chance... together.", Delay = 3f });

        dialogues.Add(new DialogueEntry { Name = "You", Text = "Yeah. Together.", Delay = 3f });
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

            // Ensure text is cleared before typing effect
            if (dialogueText != null)
            {
                dialogueText.Text = "";
                await TypeText($"{entry.Name}: {entry.Text}");
            }

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
        if (dialogueText == null) return;

        dialogueText.Text = "";
        foreach (char letter in fullText)
        {
            dialogueText.Text += letter;
            await ToSignal(GetTree().CreateTimer(0.025f), "timeout"); // Typing effect
        }
    }

    private async void EndGameSequence()
    {
        this.Visible = false;
        await ToSignal(GetTree().CreateTimer(2), "timeout");

        // Handle screen fade-out
        if (fadeOverlay != null)
        {
            fadeOverlay.Visible = true;
            Tween tween = GetTree().CreateTween();
            tween.TweenProperty(fadeOverlay, "modulate:a", 1.0f, 5.0f); // Fade to black over 5 seconds

            await ToSignal(GetTree().CreateTimer(7), "timeout");
        }
        GetTree().ChangeSceneToFile("res://GameJam/EndScreen.tscn"); // Change to the new end screen
    }
}
