using Godot;
using System;

//InventoryUI especially made for gardening
//Is likely temporary based on extremely simple game design
public partial class InventoryUIGardening : InventoryUIInteractables {

    private GardeningPatch gardeningPatch;

    public void SetGardeningPatch(GardeningPatch patch) {
        gardeningPatch = patch;
    }

    //Sends the seed the player clicked on to the gardening patch
    public override void LoadItem(int buttonIndex) {
        InventoryCell ic = FindClickedInvCell(buttonIndex);

        //set the seed in the gardening patch and plant it
        if (ic is not null) {
            gardeningPatch.PlantSeed((SeedR)ic.itemR, ic.item);
            TurnInventoryOn(false);
        }
    }



}
