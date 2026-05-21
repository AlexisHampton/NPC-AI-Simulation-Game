using Godot;
using System.Collections.Generic;

//InventoryUIBasket handles the inventory UI for baskets 
public partial class InventoryUIBasket : InventoryUIDynamicInventory {

    [Export] private Button addToInventoryButton;

    public override void _Ready() {
        base._Ready();
        addToInventoryButton.Pressed += AddItemsToPlayerInventory;
    }

    //Adds all the items to the player's inventory
    public void AddItemsToPlayerInventory() {
        foreach (InventoryCell ic in cellsUI.Keys) {
            Utilities.AddItemToPlayerInventory(player, ic.item);
        }
    }
}
