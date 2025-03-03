using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Panel4 : Panel
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
      
        dialogueText = GetNodeOrNull<Label>("Label");
        dialogueTimer = GetNodeOrNull<Timer>("Timer");

      
        fadeOverlay = GetNodeOrNull<ColorRect>("/root/GoodEnding/CanvasLayer/FadeOverlay");
   
        this.Visible = false;

        
        GenerateFinalDialogue();

      
        GetTree().CreateTimer(9).Timeout += StartFinalDialogue;
    }

  private void GenerateFinalDialogue()
    {
        dialogues.Clear();

        dialogues.Add(new DialogueEntry { Name = "You", Text = "I almost lost you...", Delay = 2f });

        dialogues.Add(new DialogueEntry { Name = "Wife", Text = "I know...", Delay = 2f });

        dialogues.Add(new DialogueEntry { Name = "You", Text = "If I had been just a little slower—", Delay = 2.5f });

        dialogues.Add(new DialogueEntry { Name = "Wife", Text = "But you weren’t. You were there when I needed you most.", Delay = 3f });

        dialogues.Add(new DialogueEntry { Name = "Wife", Text = "The doctors said another few hours and it would've been too late.", Delay = 3f });

        dialogues.Add(new DialogueEntry { Name = "You", Text = "I don’t even want to think about that.", Delay = 2f });

        dialogues.Add(new DialogueEntry { Name = "Wife", Text = "Me neither.", Delay = 2f });

        dialogues.Add(new DialogueEntry { Name = "Wife", Text = "But… I’m still here. And that’s because of you.", Delay = 3f });

        dialogues.Add(new DialogueEntry { Name = "You", Text = "Yeah... but I wish I had gotten to you sooner.", Delay = 2.5f });

        dialogues.Add(new DialogueEntry { Name = "Wife", Text = "You did everything you could. And in the end... it was enough.", Delay = 3f });

        dialogues.Add(new DialogueEntry { Name = "You", Text = "I just keep thinking... what if next time, I’m too late?", Delay = 3f });

        dialogues.Add(new DialogueEntry { Name = "Wife", Text = "Then we don’t let there be a next time.", Delay = 2.5f });

        dialogues.Add(new DialogueEntry { Name = "Wife", Text = "From now on... we face everything together.", Delay = 3f });

        dialogues.Add(new DialogueEntry { Name = "You", Text = "Yeah... together.", Delay = 3f });
    }



    private void StartFinalDialogue()
    {
        GD.Print("✅ Final dialogue started.");
        this.Visible = true;
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
        GD.Print("✅ Ending sequence started.");
        this.Visible = false;
        await ToSignal(GetTree().CreateTimer(2), "timeout");

        // Handle screen fade-out
        if (fadeOverlay != null)
        {
            fadeOverlay.Visible = true;
            Tween tween = GetTree().CreateTween();
            tween.TweenProperty(fadeOverlay, "modulate:a", 1.0f, 5.0f);

            await ToSignal(GetTree().CreateTimer(7), "timeout");
        }

        GD.Print("✅ Fade-out complete. End of scene.");
    }
}
