using Godot;
using System;

//SitTask is a Task that allows an NPC to sit on a sittable
public partial class SitTask : Task {

    [Export] Sittable sittable;

    public override bool CheckIfCanDoTask(NPC npc) {
        return base.CheckIfCanDoTask(npc);
    }

    //Claim seat so no other npc can sit here
    public override void ClaimTask(NPC npc) {
        sittable.ClaimSeat();
        base.ClaimTask(npc);
    }

    //Sit
    public override void DoTask(NPC npc) {
        if (npc.GetTaskStep == 0)
            sittable.SitDown(npc);
        base.DoTask(npc);
    }
    //Stand up from seat
    public override void FinishTask(NPC npc) {
        sittable.StandUp(npc);
        base.FinishTask(npc);
    }


}
