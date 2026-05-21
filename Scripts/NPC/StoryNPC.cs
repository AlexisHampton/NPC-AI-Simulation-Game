using Godot;
using System;

//StoryNPCs contain important world quests that move the main story along
//They will have more plot armor and will possibly not be able to be romanced
public partial class StoryNPC : NPC {

    [Export] private NPCStoryComp npcStoryComp;

    //On interact will relay story information if they can
    public override void Interact(Node3D body) {
        //if npc is a story npc and has not talked to player yet
        if (npcStoryComp is not null && !npcStoryComp.HasTalked) {
            string sym = npcStoryComp.GetRandSymbol();
            Globals.Instance.DialogueManager.StartDialogue(Name, sym);
            GameEvents.OnPlayerDialogue += HandleDialogueStopped;
        } else
            base.Interact(body);
    }


}
