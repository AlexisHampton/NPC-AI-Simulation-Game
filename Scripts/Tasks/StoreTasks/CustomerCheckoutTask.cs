using Godot;
using System;

[GlobalClass]
//CustomerCheckoutTask is a StoreTask where NPCs join the line and get checked out
public partial class CustomerCheckoutTask : StoreTask {

    //customers can always check out, because if they don't, they are thieves unintentionally
    public override bool CheckIfCanDoTask(NPC npc) {
        //return base.CheckIfCanDoTask(npc);
        return true;
    }

    //if line offset is wrong, its because its getting increased every time it's checked
    //returns a position so an npc can stand in the correct place on line
    public override Vector3 GetTaskPosition() {
        return store.GetCustSpawnOffset();
    }
    //Adds npc to line 
    public override void DoTask(NPC npc) {
        if (npc.GetTaskStep == 0) {
            store.AddCustomerToLine(npc);
        } else {
            if (!store.IsCustomerStillOnLine(npc))
                FinishTask(npc);
        }
        base.DoTask(npc);
    }

    public override void FinishTask(NPC npc) {
        //step off line
        //fix offset
        //packaway task
        npc.SetNextTask(npc.GetPackAwayTask);
        store.DecreaseCapacity();
        base.FinishTask(npc);
    }
}
