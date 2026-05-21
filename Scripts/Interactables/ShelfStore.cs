using Godot;
using System;
using System.Collections.Generic;

public partial class ShelfStore : Shelf {

    [Export] private InventoryUIBasket inventoryUIBasket;

    public override void Interact(Node3D body) {
        if (body is not Player player) return;
        List<InventoryCell> cells = dynamicInventory.GetItemsInInventory();
        inventoryUIBasket.LoadItemsIntoInvCells(cells);
        inventoryUIBasket.SetUpInventory(player, dynamicInventory);
        inventoryUIBasket.TurnInventoryOn(true);
    }
}
