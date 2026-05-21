using Godot;
using System;

//CheckOutCustomerTask is a StoreTask completed by NPCs with the cashier or adjacent job
//The NPC will remove customers from the line
public partial class CheckOutCustomerTask : StoreTask {

    public override bool CheckIfCanDoTask(NPC npc) {
        return base.CheckIfCanDoTask(npc);
    }

    //Check out customers if they're in the store
    public override void DoTask(NPC npc) {
        //check out customer if customer is in line
        GD.PrintS(npc.Name, "line empty?", store.IsLineEmpty(), store.IsOpen);
        if (!store.IsLineEmpty()) {
            store.RemoveCustomerFromLine();
        }
        base.DoTask(npc);
    }

    public override void FinishTask(NPC npc) {
        //close shop
        base.FinishTask(npc);
    }
}
