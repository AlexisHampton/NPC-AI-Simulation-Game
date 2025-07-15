using Godot;
using System;
using System.Collections.Generic;
using System.Threading;

//GardeningPatch is a simple way for the player to plant seeds
public partial class GardeningPatch : StaticBody3D, IInteractable {

    [Export] private InventoryUIGardening gardeningInvUI;

    private bool hasPlanted;
    private bool hasSeedGrown = false;
    private Player player;

    private SeedR currSeed;
    private Item currSeedItem;
    private Item finalSeedItem;
    private int daysSincePlanted;

    public override void _Ready() {
        GameEvents.OnDayIncrease += GrowSeed;
        gardeningInvUI.SetGardeningPatch(this);
        hasPlanted = false;
    }


    //Plants a seed if no seed or harvests the plant if it has grown
    public void Interact(Node3D body) {
        if (!hasPlanted && body is Player playerIn) {
            player = playerIn;
            SetUpInventory();
        }

        //harvest seed
        if (hasSeedGrown) {
            Utilities.AddItemToPlayerInventory(player, finalSeedItem);
            hasSeedGrown = false;
            hasPlanted = false;
        }
    }

    //populate inventory from player and wait for a seed to be clicked
    public void SetUpInventory() {
        player.TurnInventoryOn(false);
        GameEvents.RaisePlayerMovementStopped(false);
        gardeningInvUI.TurnInventoryOn(true);
        currSeed = null;
        currSeedItem = finalSeedItem = null;

        List<InventoryCell> seedsICs = player.Inventory.GetAllItemsByType(ItemType.SEED);

        foreach (InventoryCell cell in seedsICs)
            gardeningInvUI.AddToInventoryUI(cell);

        hasPlanted = true;
        hasSeedGrown = false;
    }

    //when player selects item via ui, plant the seed
    public void PlantSeed(SeedR seed, Item seedItem) {
        currSeed = seed;
        currSeedItem = seedItem;
        player.Inventory.RemoveFromInventory(seed, 1);

        seedItem.Reparent(this);
        seedItem.Position = Vector3.Up;
        seedItem.DisableItem(false);
    }

    //Grows the seed every day and plants its instantiated form in the patch
    private void GrowSeed(DayOfTheWeek day) {
        daysSincePlanted++;
        if (daysSincePlanted >= currSeed.DaysUntilHarvest) {
            hasSeedGrown = true;
            currSeedItem.QueueFree();
            finalSeedItem = Utilities.InstantiateItem(currSeed.FinalCrop, Vector3.Zero, this);
        }
    }
}
