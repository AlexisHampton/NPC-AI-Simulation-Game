using Godot;
using System;
//CookFoodRestaurantTask is a TavernTask where a npc cooks a recipie and puts it on the counter
public partial class CookFoodRestaurantTask : TavernTask {

    private OrderInfo currOrderInfo;

    //if no orders, don't cook
    public override bool CheckIfCanDoTask(NPC npc) {
        GD.PrintS(npc.Name, "problem task");
        if (!tavern.HasOrders()) return false;
        return base.CheckIfCanDoTask(npc);
    }

    //NPC cooks an order and puts it on the counter
    public override void DoTask(NPC npc) {
        int ts = npc.GetTaskStep % 2;

        if (ts == 0) { //spawn first order and pick it up
            if (!tavern.HasOrders()) {
                FinishTask(npc);
                return;
            }
            currOrderInfo = tavern.RemoveFirstOrder();
            Item item = currOrderInfo.order.GetPackedScene().Instantiate() as Item;
            AddChild(item);
            npc.PickUp(item);
            // go to food stock
            npc.Move(tavern.GetFoodStockPositon());
        } else if (ts == 1) {//place order on food stock
            if (!npc.AreHandsEmpty())
                tavern.AddToFoodStock(currOrderInfo, npc.PutDown(false));
        }



        base.DoTask(npc);
    }

    public override void FinishTask(NPC npc) {
        base.FinishTask(npc);
    }
}
