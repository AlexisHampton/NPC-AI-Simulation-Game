using Godot;
using System;

//CleanTableTask is a TavernTask where npcs will be able to clean a surface
//not implemented yet 
public partial class CleanTableTask : TavernTask {

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
