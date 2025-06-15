using Godot;
using System;

//TakeOrderTask is a TavernTask where an npc with the server job takes orders of npcs
public partial class TakeOrderTask : TavernTask {

    private NPC customer;

    //if there are orders to deliver, leave
    public override bool CheckIfCanDoTask(NPC npc) {
        if (tavern.HasCompletedOrders()) return false;
        return base.CheckIfCanDoTask(npc);
    }

    //get the customer's order
    public override void DoTask(NPC npc) {
        int ts = npc.GetTaskStep % 2;
        //if customer is on the line, go to them and add their order to the orders
        if (!tavern.IsLineEmpty()) {
            customer = tavern.RemoveCustomerFromLine();
            npc.Move(customer.GlobalPosition);
            tavern.AddOrder(customer);
        } else // otherwise, go stand at the counter   
            npc.Move(GlobalPosition);

        base.DoTask(npc);
    }

    public override void FinishTask(NPC npc) {
        //tavern.OpenStore(false);
        base.FinishTask(npc);
    }
}
