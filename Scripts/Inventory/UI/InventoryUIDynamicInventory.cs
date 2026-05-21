using Godot;
using System;

public partial class InventoryUIDynamicInventory : InventoryUIInteractables {

    protected Player player;
    protected DynamicInventory dynamicInventory;

    //populates player and di when in use
    public void SetUpInventory(Player playerIn, DynamicInventory dynamicInventoryIn) {
        player = playerIn;
        dynamicInventory = dynamicInventoryIn;
    }

    //Adds the inventory cell item to player's inventory
    public override void LoadItem(int buttonIndex) {
        InventoryCell ic = FindClickedInvCell(buttonIndex);

        if (ic is null) return;

        //add to player inventory
        Utilities.AddItemToPlayerInventory(player, ic.item);
        dynamicInventory.RemoveFromInventory(ic.itemR);
        RemoveFromInventory(ic);
        TurnInventoryOn(false);
    }


}
