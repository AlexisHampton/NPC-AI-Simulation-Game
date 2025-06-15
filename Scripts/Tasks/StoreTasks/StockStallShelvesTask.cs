using Godot;
using System;
using System.Collections.Generic;

//StockStallShelvesTask is a StoreTask where an npc would stock the shelves of a stall
//might be deleted in the future
public partial class StockStallShelvesTask : StoreTask {

    [Export] private Shelf shelf;

    public override bool CheckIfCanDoTask(NPC npc) {
        if (npc.IsBasketEmpty()) return false;
        return base.CheckIfCanDoTask(npc);
    }

    public override void DoTask(NPC npc) {
        if (npc.GetTaskStep == 0) {
            List<ItemInfo> itemInfos = npc.EmptyBasket();
            shelf.StockShelf(itemInfos);
            //deal with excess, the ones that don't fit
            FinishTask(npc);
        }
        base.DoTask(npc);
    }

    public override void FinishTask(NPC npc) {
        base.FinishTask(npc);
    }
}
