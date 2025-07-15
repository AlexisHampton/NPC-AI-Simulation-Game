using Godot;
using System;

//Material gatherer is a class attached to ore or wood, anything that needs to be attacked to get resources
public partial class MaterialGatherers : StaticBody3D, IHittable {

    [Export] private Item material;
    [Export] private int maxDaysUntilRespawn = 2;
    [Export] private int maxHits = 5;
    [Export] private ProgressBar progressBar;
    [Export] private MeshInstance3D progressBarUI;

    [Export] private CollisionShape3D collisionShape;

    private bool inProgress = false;
    private bool isDepleted = false;
    private int daysSinceDepleted = 0;
    private Player player;

    public override void _Ready() {
        material.DisableItem(true);
        GameEvents.OnDayIncrease += RespawnMaterial;
    }

    //When object is hit, increase hit progress or give the material to the player
    public void Hit(Node3D body) {
        //gather the material
        if (isDepleted) return;
        if (!inProgress && body is Player playerIn) {
            SetUpProgressBar();
            player = playerIn;
        }

        progressBar.Value += 1;
        GD.PrintS(progressBar.Value, progressBar.MaxValue);

        if (progressBar.Value >= maxHits)
            GivePlayerItem();

    }

    //despawns matieral and puts the item in the players inventory
    private void GivePlayerItem() {
        //despawns material
        Visible = false;
        collisionShape.Disabled = true;
        progressBarUI.Visible = false;

        //gives the new item to the player
        Item newItem = (Item)material.Duplicate();
        Utilities.AddItemToPlayerInventory(player, newItem);

        //sets up to be spawned again
        isDepleted = true;
        inProgress = false;
        daysSinceDepleted = 0;
    }

    //Resets the progress bar 
    public void SetUpProgressBar() {
        progressBar.Value = 0;
        progressBar.MaxValue = maxHits;
        progressBarUI.Visible = true;
        inProgress = true;
    }

    //Respawns the material if enough days have passed
    private void RespawnMaterial(DayOfTheWeek day) {
        daysSinceDepleted++;
        if (daysSinceDepleted < maxDaysUntilRespawn) return;

        //respawn
        isDepleted = false;
        Visible = true;
        collisionShape.Disabled = false;
    }

}
