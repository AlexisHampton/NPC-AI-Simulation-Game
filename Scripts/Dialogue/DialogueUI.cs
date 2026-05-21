using System.Linq;
using Godot;
using Godot.Collections;

//DialogueUI controls all the dialogueUI for dialogue with NPCs
public partial class DialogueUI : Control {

    [ExportGroup("Dialogue")] //dialogue UI
    [Export] private Control dialoguePanel;
    [Export] private Label nameLabel;
    [Export] private Label dialogueLabel;

    [ExportGroup("Choices")] //choice UI
    [Export] private Control choicePanel;
    [Export] private Array<Button> choiceButtons = new Array<Button>();

    [ExportGroup("LetterTimer")]
    [Export] private Timer letterTimer;
    [Export] private float timeUntilNextLetter = 0.1f;

    private Dialogue currDialogue;
    private Dictionary<string, Dialogue> currDialogueTree;


    string currSentence;
    int letterIndex = 0;
    bool handlingChoices = false;

    //On start, reset dialogue and set up choices
    public override void _Ready() {

        currDialogue = null;
        IsOn(false);

        //set up choiceButton signals
        for (int i = 0; i < choiceButtons.Count; i++) {
            choiceButtons[i].Visible = false;
            int index = i;
            choiceButtons[i].Pressed += () => PickChoices(index);
        }

        //reset letter timer
        letterTimer.WaitTime = timeUntilNextLetter;
        letterTimer.Timeout += OnLetterTimerTimeout;
        letterTimer.OneShot = true;
    }

    //Loads UI and sets dialogue tree
    public void StartDialogue(string name, Dictionary<string, Dialogue> dialogue, string startID) {
        IsOn(true);
        currDialogueTree = dialogue;
        nameLabel.Text = name;
        LoadDialogue(dialogue, startID);
    }

    //Loads a one-liner dialogue
    public void StartDialogue(string name, string dialogue) {
        IsOn(true);
        nameLabel.Text = name;
        LoadDialogue(dialogue);
    }

    //Turns on the UI and resets everything
    private void IsOn(bool isOn) {
        dialoguePanel.Visible = isOn;
        choicePanel.Visible = false;
        nameLabel.Text = dialogueLabel.Text = currSentence = "";
        letterIndex = 0;
        handlingChoices = false;

        if (!isOn) currDialogue = null;
        GameEvents.RaisePlayerDialogue(isOn);
    }

    //Continues to the next dialogue ID or ends dialogue
    public void ContinueDialogue() {

        //if at end of dialogue, end dialogue
        if (currDialogue is null) {
            EndDialougue();
            return;
        }
        //load next dialogue
        LoadDialogue(currDialogueTree);

    }

    //Turns off dialogue menu
    public void EndDialougue() {
        IsOn(false);
        currDialogue = null;
    }

    //Loads dialogue for the startID or loads choices
    public void LoadDialogue(Dictionary<string, Dialogue> dialogue, string startID = "start") {
        if (handlingChoices) {
            HandleChoices(currDialogue);
            return;
        }
        //if no current dialogue, load the start
        if (currDialogue == null)
            currDialogue = dialogue[startID];

        //the rest of the time, load curr dialogue
        LoadDialogue(currDialogue.text);

        //if choices load them here
        if (currDialogue != null && currDialogue.options != null) {
            handlingChoices = true;
            return;
        }

        //if at end of dialogue
        if (currDialogue.goTo is null) {
            currDialogue = null;
            return;
        }
        //otw load next dialogue
        currDialogue = dialogue[currDialogue.goTo[0]];
    }

    //Load the dialogue word by word
    public void LoadDialogue(string dialogue) {
        dialogueLabel.Text = "";
        currSentence = dialogue;
        letterIndex = 0;
        dialogueLabel.Text += currSentence[0];
        letterTimer.Start();
    }

    //Loads each letter to the screen
    public void OnLetterTimerTimeout() {
        letterIndex++;
        if (letterIndex >= currSentence.Length)
            return;
        dialogueLabel.Text += currSentence[letterIndex];
        letterTimer.Start();
    }

    //Loads choices to the screen
    public void HandleChoices(Dialogue dialogue) {
        handlingChoices = true;
        //load choices to ui
        for (int i = 0; i < dialogue.options.Length; i++) {
            choiceButtons[i].Text = dialogue.options[i];
            choiceButtons[i].Visible = true;
        }
        choicePanel.Visible = true;
    }

    //Loads the dialogue for the choice picked
    private void PickChoices(int choiceIndex) {
        handlingChoices = false;
        foreach (Control cb in choiceButtons)
            cb.Visible = false;
        choicePanel.Visible = false;

        //weird glitch, think it's double clicking that causes this
        if (currDialogue is null || currDialogue.goTo is null) return;

        string id = currDialogue.goTo[choiceIndex];
        currDialogue = currDialogueTree[id];
        LoadDialogue(currDialogueTree, id);
    }
}
