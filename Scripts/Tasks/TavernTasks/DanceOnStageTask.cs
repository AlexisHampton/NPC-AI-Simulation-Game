using Godot;
using System;

//DanceOnStageTask is a TavernTask where an npc would dance as a jobTask
public partial class DanceOnStageTask : TavernTask {

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
