using Godot;
using System;

//PartnerTask is a Task that takes more than one person
//more used like an abstract class since most of what would be here is in NPC.NextTask()
public partial class PartnerTask : Task {

    [Export] private bool canUseOriginalPosition = true;
    NPC passiveNPC;


    public override bool CheckIfCanDoTask(NPC npc) {
        return base.CheckIfCanDoTask(npc);
    }

    //returns the position of the task plus an offset
    public override Vector3 GetTaskPosition() {
        //if (canUseOriginalPosition) return base.GetTaskPosition();
        return base.GetTaskPosition() + new Vector3(1, 0, 1);
        //return passiveNPC.GlobalPosition + new Vector3(2, 0, 2);
    }

    public void ClaimTask(NPC activeNPC, NPC passiveNPCIn) {
        passiveNPC = passiveNPCIn;
    }

    public override void DoTask(NPC npc) {

        base.DoTask(npc);
    }

    public virtual void ActionNPCDoTask(NPC npc) {
        GD.PrintS(npc, "is active NPC");
    }

    public virtual void PassiveNPCDoTask(NPC npc) {
        GD.PrintS(npc, "is passsive NPC");
    }

    public override void FinishTask(NPC npc) {
        base.FinishTask(npc);
    }
}
