using Godot;
using System;

//The specific inventory UI for the player that only turns on via button press
public partial class InventoryUIPlayer : InventoryUI {

    //Make the inventory and mouse visible or invisible
    public override void TurnInventoryOn(bool isOn) {
        base.TurnInventoryOn(isOn);
        if (isOn)
            Input.MouseMode = Input.MouseModeEnum.Visible;
        else
            Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    //Waits for inventory input ('I' or 'B' key) to turn on/off inventoryPanel
    public override void _Input(InputEvent @event) {
        if (@event.IsActionPressed("inventory"))
            TurnInventoryOn(!Visible);


    }
}
