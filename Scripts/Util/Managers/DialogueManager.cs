using Godot;
using System.Collections.Generic;
using Godot.Collections;

public partial class DialogueManager : Node {

    [ExportGroup("Dialogue Files")]
    [Export] public Array<DialogueFileRelation> ChatDialogueFiles { get; private set; } = new();
    [Export] public DialogueFileR StoryDialogue { get; private set; }
    [Export] public DialogueFileR GiftDialogue { get; private set; }
    [Export] public DialogueFileR AskDialogue { get; private set; }
    [Export] public DialogueFileR InsultDialogue { get; private set; }
    [Export] public DialogueFileR NPCChatDialogue { get; private set; }



    [ExportGroup("UI")]
    [Export] private DialogueUI dialogueUI;
    [Export] private DialogueChoicesUI dialogueChoicesUI;

    //Parse all dialogue files for use
    public override void _Ready() {
        ParseDialogueFile(StoryDialogue);
        ParseDialogueFile(AskDialogue);
        ParseDialogueFile(GiftDialogue);
        ParseDialogueFile(InsultDialogue);
        ParseDialogueFile(NPCChatDialogue);


        //parse all the dialogue files and store the start symbols
        foreach (DialogueFileRelation dfr in ChatDialogueFiles)
            ParseDialogueFile(dfr);
    }

    public static void ParseDialogueFile(DialogueFileR dfr) {
        dfr.DialogueTree = JSONDialogueParser.GetDialogue(dfr.dialogueFile);
        dfr.StartSymbols = [.. dfr.DialogueTree.Keys];
    }

    //Starts the Story NPC dialogue with the startID provided and dialogue Tree
    public void StartDialogue(string name, string startID, Godot.Collections.Dictionary<string, Dialogue> dialogueTree) {
        dialogueUI.StartDialogue(name, dialogueTree, startID);
    }

    //Starts the Story NPC dialogue with the startID provided
    public void StartDialogue(string name, string startID) {
        dialogueUI.StartDialogue(name, StoryDialogue.DialogueTree, startID);
    }

    //Starts the Regular NPC dialogue which picks a startID
    public void StartDialogue(NPC npc) {
        dialogueChoicesUI.IsOn(true, npc);
    }

    //starts a npc driven dialogue
    public void StartDialogueNPC(NPC npc) {
        dialogueChoicesUI.StartNPCDialogue(npc, NPCChatDialogue);
    }


}
