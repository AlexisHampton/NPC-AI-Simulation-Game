using Godot;
using System;

//An inventory a Player or object can hold that also renders the objects to the screen
public partial class InventoryStatic : Inventory {

    [Export] private InventoryUI inventoryUI;

    //check if all ui slots are taken
    public override bool IsFull() {
        return inventoryUI.IsFull();
    }

    //Add an item to inventory and then render it to the screen
    public override void AddToInventory(Item item, int amt) {
        base.AddToInventory(item, amt);

        //get item 
        InventoryCell ic = inventoryCells[item.ItemR];
        inventoryUI.AddToInventoryUI(ic);
    }

    //Removes an item from the inventory and takes it off the screen
    public override InventoryCell RemoveFromInventory(ItemR item, int amt) {
        InventoryCell ic = base.RemoveFromInventory(item, amt);

        if (inventoryCells.ContainsKey(item))
            inventoryUI.AddToInventoryUI(ic);
        else
            inventoryUI.RemoveFromInventory(ic);

        return ic;

    }

    //Turns the inventory on or off
    public void TurnInventoryOn(bool isOn) {
        inventoryUI.TurnInventoryOn(isOn);
    }


}
