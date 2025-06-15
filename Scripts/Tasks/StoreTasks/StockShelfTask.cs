using Godot;
using System;

//StockShelfTask is a StoreTask where an npc with the stocker job stocks shelves with items from crates
public partial class StockShelfTask : StoreTask {

    [Export] private DynamicInventory stock;

    private Shelf shelf;
    private Crate crateToStock;

    //if there is no stock or empty shelves, don't stock
    public override bool CheckIfCanDoTask(NPC npc) {
        shelf = store.GetAnEmptyShelf();
        if (!store.HasStock()) return false; // no stock
        if (shelf is null) return false; // no empty shelves
        return base.CheckIfCanDoTask(npc);
    }

    //NPC stocks the shelves
    public override void DoTask(NPC npc) {
        //just in case null check
        if (shelf is null) {
            FinishTask(npc);
            return;
        }
        switch (npc.GetTaskStep) {
            //get stock from shelf
            case 0:
                GD.PrintS(npc.Name, "stocking again", shelf);

                ItemInfo item = store.RemoveFromStock(shelf.GetStockedItem);
                npc.PickUp(item.item);
                break;
            //go to the shelf
            case 1:
                npc.Move(shelf.GlobalPosition);
                break;
            //stock the shelf
            case 2:
                crateToStock = (Crate)npc.PutDown(false);
                shelf.StockShelf(crateToStock);
                break;
            //delete the crate
            case 3:
                crateToStock.QueueFree();
                FinishTask(npc);
                break;
        }

        base.DoTask(npc);
    }

    public override void FinishTask(NPC npc) {
        base.FinishTask(npc);
    }
}
