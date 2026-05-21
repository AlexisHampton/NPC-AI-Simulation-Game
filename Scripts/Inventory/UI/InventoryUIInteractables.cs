using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

//Inventory UI for inventories of chests, shelves, anything interactable
//Will be implemented later
public partial class InventoryUIInteractables : InventoryUI {

    [Export] private Button xButton;

    //Allows the xButton to function
    public override void _Ready() {
        base._Ready();
        xButton.Pressed += HandleXButtonPressed;
    }

    //Loads the indicated items into the inventoryUI
    public virtual void LoadItemsIntoInvCells(List<InventoryCell> cells) {
        foreach (InventoryCell cell in cells)
            AddToInventoryUI(cell);
    }

    //Does something to the inventory cell that was clicked
    public override void LoadItem(int buttonIndex) { }

    //returns the inventory cell that corresponds to the button that was clicked on
    protected InventoryCell FindClickedInvCell(int buttonIndex) {
        InventoryCellUI cellUI = inventoryCellUIs[buttonIndex];
        InventoryCell ic = null;

        foreach (InventoryCell invCell in cellsUI.Keys)
            if (cellsUI[invCell] == cellUI) {
                ic = invCell;
                break;
            }

        return ic;
    }


    //override since there are no item names or descriptions to reset in this ui so far
    public override void TurnInventoryOn(bool isOn) {
        Visible = isOn;
        GameEvents.RaiseStopPlayerMovement(isOn);

    }


}
