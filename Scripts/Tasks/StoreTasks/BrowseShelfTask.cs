using Godot;
using System;

//BrowseShelfTask is a StoreTask that allows NPCs to look for items to buy in a store
public partial class BrowseShelfTask : StoreTask {

    [Export] private Shelf shelf;

    //if store isn't open, there are no items or it's too crowded, leave
    public override bool CheckIfCanDoTask(NPC npc) {
        if (!store.GetIsOpen) return false;
        if (shelf.IsEmpty()) return false;
        if (store.IsStoreFull()) return false;
        //if (store.AddCapacity()) return false;

        return base.CheckIfCanDoTask(npc);
    }

    //NPC has a random chance to get an item
    public override void DoTask(NPC npc) {
        if (npc.GetTaskStep == 0 && !shelf.IsEmpty()) {
            store.AddCapacity();
            int randChance = GD.RandRange(1, 1); //set to 1 for testing purposes
            if (randChance == 1) {
                InventoryCell ic = shelf.TakeRandomItem(1);
                npc.AddToBasket(ic.item, ic.itemAmount);
            }
        } else if (npc.GetTaskStep >= 1)
            FinishTask(npc);

        base.DoTask(npc);
    }

    //NPC has a random chance to check out or browse another shelf
    public override void FinishTask(NPC npc) {
        int randChance = GD.RandRange(0, 10);

        if (!store.GetIsOpen)
            npc.SetNextTask(store.GetCustomerCheckoutTask);
        //60% chance to checkout
        if (npc.IsBasketFull() || randChance >= 4)
            npc.SetNextTask(store.GetCustomerCheckoutTask);
        //if can't, continue browsing or checkout if the store is full
        else {
            BrowseShelfTask bst = store.GetBrowseTask(npc);
            if (bst != null)
                npc.SetNextTask(bst);
            else
                npc.SetNextTask(store.GetCustomerCheckoutTask);
        }
        //store.DecreaseCapacity();
        base.FinishTask(npc);
    }
}
