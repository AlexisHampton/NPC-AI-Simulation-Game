using Godot;
using System;
using Godot.Collections;

public partial class DeliverToStoreTask : StoreTask {

    [Export] private Array<Store> storesToDeliverTo = new Array<Store>();

    private int currStore = -1;

    public override bool CheckIfCanDoTask(NPC npc) {
        if (storesToDeliverTo.Count == 0) return false;
        if (!npc.GetCart.HasItems()) return false;
        return base.CheckIfCanDoTask(npc);
    }

    //return the current stores trader delivery position
    public override Vector3 GetTaskPosition() {
        if (storesToDeliverTo.Count == 0 || currStore < 0)
            return base.GetTaskPosition();
        return storesToDeliverTo[currStore].GetTraderDeliveryPoint.GlobalPosition;
    }

    //resets path of stores
    public override void ClaimTask(NPC npc) {
        base.ClaimTask(npc);
        currStore = 0;
    }

    //Drops off the items to the stores
    public override void DoTask(NPC npc) {
        int ts = npc.GetTaskStep % 2; //2 steps
        if (currStore >= storesToDeliverTo.Count) {
            base.DoTask(npc);
            return;
        }

        switch (ts) {
            case 0: // drop off the goods
                //anim
                if (!npc.GetCart.HasItems()) {
                    FinishTask(npc);
                    break;
                }
                npc.SetTaskStep = -1;
                ItemInfo randItem = npc.GetCart.RemoveOneItem();
                storesToDeliverTo[currStore].AddToStock(randItem.itemR, randItem.item);
                break;
            case 1: //go to next store
                currStore++;
                if (currStore >= storesToDeliverTo.Count) {

                    FinishTask(npc);
                    break;
                }
                npc.Move(storesToDeliverTo[currStore].GetTraderDeliveryPoint.GlobalPosition);
                break;
        }

        base.DoTask(npc);
    }

    public override void FinishTask(NPC npc) {
        base.FinishTask(npc);
    }
}
