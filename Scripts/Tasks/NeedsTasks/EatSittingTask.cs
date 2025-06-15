using Godot;
using System;

//EatSittingTask is a task the NPC preforms after SitAndOrder task
//The NPC eats after ordering 
public partial class EatSittingTask : Task {

    private Sittable chair;
    private Item itemToEat;

    private int maxTimeSinceServed = 7;

    //sets a dish in front of npc
    public void SetItemToEat(Item item) {
        itemToEat = item;
    }

    //claims the chair
    public void SetChair(Sittable sittable) {
        chair = sittable;
    }

    //if no food to eat, can't eat
    public override bool CheckIfCanDoTask(NPC npc) {
        if (itemToEat is null) return false;
        return base.CheckIfCanDoTask(npc);
    }

    //NPC "eats" 
    public override void DoTask(NPC npc) {
        //if npc waits too long, leave
        if (npc.GetTaskStep > maxTimeSinceServed) {
            GD.PrintS(npc.Name, "leaving tavern because ", npc.GetTaskStep, "time since served");
            FinishTask(npc);
            return;
        }

        base.DoTask(npc);
    }

    //Delete th food and stand the npc up
    public override void FinishTask(NPC npc) {
        //if item has not been deleted yet, delete it
        if (IsInstanceValid(itemToEat))
            itemToEat.QueueFree();
        itemToEat = null;
        chair.StandUp(npc);
        base.FinishTask(npc);
    }
}
