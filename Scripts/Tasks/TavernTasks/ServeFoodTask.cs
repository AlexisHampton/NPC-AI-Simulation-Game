using Godot;
using System;

//ServeFoodTask is a TavernTask where an npc with the server job serves tavern-customers the food they ordered
public partial class ServeFoodTask : TavernTask {

    private OrderInfo currOrder;

    //if no food was made, leave
    public override bool CheckIfCanDoTask(NPC npc) {
        if (!tavern.HasCompletedOrders()) return false;
        return base.CheckIfCanDoTask(npc);
    }

    //return the counter's position
    public override Vector3 GetTaskPosition() {
        return tavern.GetFoodStockPositon();
    }

    // pick up the food and bring it to the npc who ordered it
    public override void DoTask(NPC npc) {
        int ts = npc.GetTaskStep % 2;
        if (ts == 0) { //pick up food
            if (!tavern.HasCompletedOrders()) {
                FinishTask(npc);
                return;
            }
            currOrder = tavern.RemoveFirstCompletedOrder();
            npc.PickUp(tavern.RemoveFromFoodStock(currOrder.order));
            //go to npc
            npc.Move(currOrder.customer.GlobalPosition);
        } else if (ts == 1) { //transfer dish to item

            if (currOrder.customer.GetCurrTask is SitAndOrderTask sitAndOrderTask) {
                sitAndOrderTask.SetFood(npc.PutDown(false));
            }
            // go back to counter pos
            npc.Move(GlobalPosition);

            if (!tavern.HasCompletedOrders()) {
                FinishTask(npc);
                return;
            }
        }
        base.DoTask(npc);
    }

    public override void FinishTask(NPC npc) {
        base.FinishTask(npc);

    }
}
