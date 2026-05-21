using Godot;
using Godot.Collections;
using System.Collections.Generic;

//Handles the Dialogue Options for the player to control the flow of conversation
public partial class DialogueChoicesUI : Control {

    [ExportGroup("Panels")]
    [Export] private Control dialogueChoicesPanel;
    [Export] private Control askOptionsPanel;
    [Export] private Control chatOptionsPanel;

    [ExportGroup("Buttons")]
    [Export] private Array<Button> askOptionButtons = new();

    [ExportGroup("Timers")]
    [Export] private Timer npcLeaveTimer; //timer that counts 10 seconds before npc leaves for no activity
    [Export] private Timer npcRelationshipLevelTimer;

    private NPC currNPC;

    //On load, hides the dialogue Options UI
    public override void _Ready() {
        IsOn(false);
        npcLeaveTimer.Timeout += NPCLeaves;
        npcRelationshipLevelTimer.Timeout += NPCTurnsOffRelationshipLabel;

        for (int i = 0; i < askOptionButtons.Count; i++) {
            int index = i;
            askOptionButtons[i].Pressed += () => AskButtonPressed(index);
        }
    }

    //Shows the dialogue UI and stops them from moving if on
    public void IsOn(bool isOn) {
        dialogueChoicesPanel.Visible = chatOptionsPanel.Visible = isOn;
        askOptionsPanel.Visible = false;
        GameEvents.RaiseStopPlayerMovement(isOn);
    }

    //Shows dialogue UI and records the NPCs name
    public void IsOn(bool isOn, NPC npc) {
        currNPC = npc;
        npcLeaveTimer.Start();
        IsOn(isOn);
    }

    //Loads dialogue for an npc starting a conversation with the player
    public void StartNPCDialogue(NPC npc, DialogueFileR dfr) {
        IsOn(false, npc);
        GameEvents.RaiseStopPlayerMovement(true);
        LoadDialogue(dfr.StartSymbols, dfr.DialogueTree, NPCManager.NPCChatRelationshipIncreaser);
    }

    //Loads dialogue for a random chat start symbol  based on relationship status
    public void Chat() {
        DialogueFileRelation dfr = Utilities.GetBestDialogueTree(Globals.Instance.DialogueManager.ChatDialogueFiles, currNPC);
        LoadDialogue(dfr.StartSymbols, dfr.DialogueTree, NPCManager.ChatRelationshipIncreaser);
    }

    //Loads dialogue for a random insult start symbol  
    public void Insult() {
        DialogueFileR insultD = Globals.Instance.DialogueManager.InsultDialogue;
        List<string> startSymbols = insultD.StartSymbols;
        LoadDialogue(startSymbols, insultD.DialogueTree, NPCManager.InsultRelationshipIncreaser);

    }

    //Gives npc the gift in the player's Hands
    public void Gift() {
        if (Globals.Instance.Player.AreHandsEmpty()) return;

        //give the gift
        Item item = Globals.Instance.Player.PutDown(false);
        currNPC.AddToBasket(item);
        currNPC.SetNextTask(currNPC.GetPackAwayTask);

        //Load gift dialogue
        DialogueFileR giftD = Globals.Instance.DialogueManager.GiftDialogue;
        LoadDialogue(giftD.StartSymbols, giftD.DialogueTree, NPCManager.GiftRelationshipIncreaser);
    }

    //show the player questions they can ask
    public void Ask() {
        chatOptionsPanel.Visible = false;
        npcLeaveTimer.Stop();

        //fill options with 5 random symbols
        DialogueFileR askD = Globals.Instance.DialogueManager.AskDialogue;
        foreach (Button b in askOptionButtons) {
            int randNum = GD.RandRange(0, askD.StartSymbols.Count - 1);
            b.Text = askD.StartSymbols[randNum];
        }

        askOptionsPanel.Visible = true;
    }

    //Loads the start symbol connected to the button that was pressed
    public void AskButtonPressed(int symbolIndex) {
        string startSymbol = askOptionButtons[symbolIndex].Text;
        LoadDialogue(startSymbol, Globals.Instance.DialogueManager.AskDialogue.DialogueTree, NPCManager.AskRelationshipIncreaser);
    }

    //Loads a specified dialogueTree with a random start symbol
    private void LoadDialogue(List<string> startSymbols, Godot.Collections.Dictionary<string, Dialogue> dialogueTree, float relationshipAmt) {
        npcLeaveTimer.Stop();
        int randNum = GD.RandRange(0, startSymbols.Count - 1);
        string randStartSymbol = startSymbols[randNum];

        LoadDialogue(randStartSymbol, dialogueTree, relationshipAmt);
    }

    //Loads a specified dialogueTree with a specified start symbol
    private void LoadDialogue(string startSymbol, Godot.Collections.Dictionary<string, Dialogue> dialogueTree, float relationshipAmt) {
        dialogueChoicesPanel.Visible = false;
        Globals.Instance.DialogueManager.StartDialogue(currNPC.Name, startSymbol, dialogueTree);

        //increase relationship level with npc
        currNPC.IncreasePlayerRelationship(relationshipAmt);
        npcRelationshipLevelTimer.Start();
    }

    //if 10 seconds pass without activity, npc will leave
    public void NPCLeaves() {
        Leave();
        GameEvents.RaisePlayerDialogue(false);
        currNPC = null;
    }

    //shuts off the relationshipLabel after a few seconds
    public void NPCTurnsOffRelationshipLabel() {
        currNPC.TurnOffRelationshipLabel();
        //  currNPC = null;
    }

    //Closes dialogueUI
    public void Leave() {
        IsOn(false, null);
    }

}
