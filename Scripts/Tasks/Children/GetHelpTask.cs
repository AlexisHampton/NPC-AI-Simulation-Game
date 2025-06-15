using Godot;
using System;

//A Task attached to an NPCChild that an NPC can sense and "help" the child
public partial class GetHelpTask : Task {

    [Export] private HelpChildTask kidSleepTask;
    [Export] private HelpChildTask batheKidTask;
    [Export] private HelpChildTask feedKidTask;

    private NPC parent;
    private Need lowestNeed;


    public void SetParentAndNeed(Need lowest, NPC activeParent) {
        parent = activeParent;
        lowestNeed = lowest;
    }

    //Sets the task for both parent and child based on child's lowest need
    public override void DoTask(NPC npc) {
        if (npc.GetTaskStep != 0) return;

        npc.FollowCharacter(parent);
        switch (lowestNeed) {
            case Need.EAT:
                feedKidTask.SetChild((NPCChild)npc);
                parent.SetNextTask(feedKidTask);
                break;
            case Need.REST:
                kidSleepTask.SetChild((NPCChild)npc);
                parent.SetNextTask(kidSleepTask);
                break;
            case Need.HYGIENE:
                batheKidTask.SetChild((NPCChild)npc);
                parent.SetNextTask(batheKidTask);
                break;
        }


        base.DoTask(npc);
    }


}
