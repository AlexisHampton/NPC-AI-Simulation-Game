using Godot;
using System;

[GlobalClass]

//A Task on a NPCChild that an NPC can "help" them do, i.e. bathing
public partial class HelpChildTask : Task {

    [Export] private Task nextChildTask;

    private NPCChild child;

    public void SetChild(NPCChild npcChild) {
        child = npcChild;
    }

    public override void DoTask(NPC npc) {
        if (child is null) return;
        if (child.GetCurrTask is not HelpChildTask)
            child.GetCurrTask.FinishTask(child);
        child.SetNextTask(nextChildTask);
        child.StopFollow();
        base.DoTask(npc);
    }

    public override void FinishTask(NPC npc) {
        child = null;
        base.FinishTask(npc);
    }
}
