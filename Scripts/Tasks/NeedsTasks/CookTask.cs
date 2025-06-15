using Godot;
using System;

//CookTask is a Task that will eventually allow NPCs to make meals using the cooking system
public partial class CookTask : Task {

    public override bool CheckIfCanDoTask(NPC npc) {
        return base.CheckIfCanDoTask(npc);
    }

    public override void DoTask(NPC npc) {
        base.DoTask(npc);
    }

    public override void FinishTask(NPC npc) {
        base.FinishTask(npc);
    }
}
