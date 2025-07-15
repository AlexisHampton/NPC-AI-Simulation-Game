using Godot;
using Godot.Collections;

//Fishing pole is a placeholder-y way of adding the fishing skill 
//Will likely be deprecated with a more complex game design in the future
public partial class FishingPole : StaticBody3D, IInteractable {
    [Export] private Timer fishingTimer;
    [Export] private Array<Item> fishToCatch;

    private bool isFishing = false;
    private Player player;

    public override void _Ready() {
        fishingTimer.Timeout += CatchFish;

        foreach (Item item in fishToCatch)
            item.DisableItem(true);
    }

    //Starts fishing
    public void Interact(Node3D body) {
        if (player is null && body is Player playerIn)
            player = playerIn;
        if (isFishing) return;

        GameEvents.RaisePlayerMovementStopped(false);

        isFishing = true;
        fishingTimer.Start();
        GD.PrintS("Starting to fish");

    }

    //When the timer runs out, gives a random fish to the player and lets them move again
    private void CatchFish() {
        isFishing = false;
        int randNum = GD.RandRange(0, fishToCatch.Count - 1);
        Item newFish = (Item)fishToCatch[randNum].Duplicate();
        Utilities.AddItemToPlayerInventory(player, newFish);

        GD.PrintS("Caught a fish: ", newFish.ItemR.ItemName, "1");
        GameEvents.RaisePlayerMovementStopped(true);
    }


}
