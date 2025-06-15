using Godot;
using System;

//PackAwayTask lets npcs put away items they bought
public partial class PackAwayTask : Task {

    [Export] private Shelf shelf;

    //if the storage is full and npc has nothing to pack, leave
    public override bool CheckIfCanDoTask(NPC npc) {
        if (shelf.IsFull()) return false;
        if (npc.IsBasketEmpty()) return false;

        return base.CheckIfCanDoTask(npc);
    }

    //stock the shelf
    public override void DoTask(NPC npc) {
        if (npc.GetTaskStep == 0) {
            shelf.StockShelf(npc.EmptyBasket());
        }
        base.DoTask(npc);
    }

    public override void FinishTask(NPC npc) {
        base.FinishTask(npc);
    }
}
