using Godot;
using System;

//DanceForFunTask is a TavernTask where npcs dance
//not fully implemented yet
public partial class DanceForFunTask : TavernTask {

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
