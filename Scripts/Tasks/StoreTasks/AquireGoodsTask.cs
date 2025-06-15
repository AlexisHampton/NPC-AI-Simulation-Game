using Godot;
using System;
using Godot.Collections;

//AquireGoodsTask is a StoreTask where 
//can later be expanded to buying from stores
public partial class AquireGoodsTask : StoreTask {

    NPC taskNPC;

    //if NPC doesn't have a cart, they can't get goods
    public override bool CheckIfCanDoTask(NPC npc) {
        //check if don't have stock
        //if (store.CheckIfHaveStock()) return false;
        if (npc.GetCart == null) return false;
        taskNPC = npc;
        return base.CheckIfCanDoTask(npc);
    }

    //return the position of the seat the NPC will sit in
    public override Vector3 GetTaskPosition() {
        return taskNPC.GetCart.GetSeatLocation.GlobalPosition;
    }

    //The NPC goes to aquire the goods and then disappears
    public override void DoTask(NPC npc) {

        switch (npc.GetTaskStep) {
            case 0:
                //Sit in the cart and move to the position of the task
                npc.GetCart.TurnOffCollision(false);
                npc.GetCart.SitInSeat(npc);
                npc.GiveRemoteControl(true);
                npc.TurnOnCollision(false);
                npc.GetCart.Move(GlobalPosition);
                break;
            case 1:
                //disappear
                npc.Visible = false;
                npc.GetCart.Visible = false;
                break;
            case 2:
                //end task
                FinishTask(npc);
                break;
        }

        base.DoTask(npc);
    }

    //Returns the items the npc "aquired"
    public Godot.Collections.Dictionary<ItemR, int> PickNumItemsToAquire() {
        //maybe #ofItems dep on job level
        int numItems = 50;
        Godot.Collections.Dictionary<ItemR, int> cratesToPack = new Godot.Collections.Dictionary<ItemR, int>();

        //List<ItemR> itemsToAquire = new List<ItemR>();
        Array<ItemR> itemsToSell = store.GetItemsToSell;


        //duplicated allowed, allows for funnier stories
        for (int i = 0; i < numItems; i++) {
            int randItem = GD.RandRange(0, itemsToSell.Count - 1);
            ItemR item = itemsToSell[randItem];
            //itemsToAquire.Add(item);
            if (cratesToPack.ContainsKey(item))
                cratesToPack[item] += 1;
            else
                cratesToPack.Add(item, 1);

        }
        return cratesToPack;
    }

    //Fill the cart with the items and make visible
    public override void FinishTask(NPC npc) {
        //aquire goods?
        Godot.Collections.Dictionary<ItemR, int> itemsToAcquire = PickNumItemsToAquire();

        npc.GetCart.PackDriveable(itemsToAcquire);
        npc.Visible = true;
        npc.GetCart.Visible = true;
        base.FinishTask(npc);
    }
}
