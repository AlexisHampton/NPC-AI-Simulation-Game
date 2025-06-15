using Godot;
using System;
using Godot.Collections;

//TalkTask is a PartnerTask that allows NPCs to "talk" to each other
public partial class TalkTask : PartnerTask {

    //npcs talking
    private Array<NPC> npcsDoingTask = new Array<NPC>();
    SocialAction socialAction;

    //Add npcs to the social circle
    public override void ClaimTask(NPC npc) {
        base.ClaimTask(npc);
        if (!npcsDoingTask.Contains(npc))
            npcsDoingTask.Add(npc);
    }

    //NPCs talk to each other
    public override void DoTask(NPC npc) {
        //if not enough npcs, return 
        if (npcsDoingTask.Count < 2) {
            FinishTask(npc);
            return;
        }
        //if the npc doing the task is the one who initiated the task
        if (npc.GetTaskStep == 0 && npc == npcsDoingTask[0]) {
            //pick a social task
            NPCRelationship npcRelationship = npc.GetNPCRelationship(npcsDoingTask[1]);

            //for strained
            if (npcRelationship is not null && npcRelationship.relationshipStatus < RelationshipStatus.STRANGER)
                socialAction = GetSocialAction(RelationshipStatus.STRAINED);
            //for lovers
            else if (npcRelationship is not null && npcRelationship.relationshipStatus >= RelationshipStatus.LOVER)
                socialAction = GetSocialAction(RelationshipStatus.LOVER);
            else //for friendly
                socialAction = GetSocialAction(RelationshipStatus.STRANGER);

            if (socialAction is null) //if can't find one, set social task to get to know
                socialAction = Globals.Instance.AllSocialActions[RelationshipStatus.STRANGER].allSocialActions[0];

            //preform it
            foreach (NPC npcDoingTask in npcsDoingTask)
                npcDoingTask.UpdateTaskLabel(socialAction.actionName);
        }
        base.DoTask(npc);
    }

    //returns a random social action based on the relationship status of the npc
    public SocialAction GetSocialAction(RelationshipStatus relationshipStatus) {
        SocialActionList socialActionList = Globals.Instance.AllSocialActions[relationshipStatus];
        int randNum = GD.RandRange(0, socialActionList.allSocialActions.Count - 1);
        return socialActionList.allSocialActions[randNum];
    }

    //Updates npc relationships after talking and resets the social circle
    public override void FinishTask(NPC npc) {
        if (socialAction is not null)
            foreach (NPC other in npcsDoingTask) {
                if (other != npc)
                    npc.UpdateNPCRelationship(other, socialAction.relationshipAmount);
            }
        npcsDoingTask.Clear();
        base.FinishTask(npc);
    }


}
