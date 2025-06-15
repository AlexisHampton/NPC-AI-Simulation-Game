using Godot;
using System;

//CookWaitTask is a TavernTask where an npc waits to cook
//implemented to keep npcs in the kitchen while at work instead of being at home. 
public partial class CookWaitTask : TavernTask {

    //if there are orders dont do this task
    public override bool CheckIfCanDoTask(NPC npc) {
        if (tavern.HasOrders()) return false;
        return base.CheckIfCanDoTask(npc);
    }

    public override void DoTask(NPC npc) {
        //tavern.OpenStore(true);
        base.DoTask(npc);
    }


}
