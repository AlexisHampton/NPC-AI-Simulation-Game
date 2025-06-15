using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;

//Driveable moves agents on the Cart Navmesh
public partial class Driveable : CharacterBody3D {

    [Export] private Array<Sittable> seats;
    [Export] private PackedScene crateScene;
    [Export] private DynamicInventory dynamicInventory;

    [ExportGroup("Movement")]
    [Export] private NavigationAgent3D agent;
    [Export] private float speed = 10;
    [Export] private float rotationSpeed;
    [Export] private Array<CollisionShape3D> collisions;

    public Node3D GetSeatLocation { get => seats[0]; }

    private Rid navMapRID;
    private Vector3 pastVel = new Vector3(-10000, -10000, -10000);

    public override void _Ready() {
        CallDeferred(MethodName.WaitForNavigationSync);
    }

    //Waits for first physics process frame before querying navigation mesh. 
    public async void WaitForNavigationSync() {
        SetPhysicsProcess(false);
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
        SetPhysicsProcess(true);

        navMapRID = NavigationPaths.Instance.GetNavigationMapDriveable;
        NavigationPaths.Instance.OnNavigationMapCreated += () => agent.SetNavigationMap(navMapRID);

    }

    //Every physics frame, keep heading toward the target position
    public override void _PhysicsProcess(double delta) {

        if (agent.IsNavigationFinished())
            return;

        Vector3 nextLoc = agent.GetNextPathPosition();
        Vector3 offset = nextLoc - GlobalPosition;
        Vector3 newVel = offset.Normalized() * speed;


        if (Velocity == pastVel) {
            GlobalPosition = nextLoc;
            return;
        }

        agent.Velocity = newVel;
        pastVel = Velocity;

        //rotation
        offset.Y = 0;
        if (GlobalTransform.Origin.IsEqualApprox(GlobalPosition + offset))
            return;
        LookAt(GlobalPosition + offset, Vector3.Up);
    }

    //Moves to a specified position . 
    public void Move(Vector3 pos) {
        //for some reason, doesn't move unless we set this again.
        agent.SetNavigationMap(navMapRID);
        agent.TargetPosition = pos;
    }

    //Turns all collisions on or off depending on input
    public void TurnOffCollision(bool turnOff) {
        foreach (CollisionShape3D c in collisions)
            c.Disabled = turnOff;
    }

    //Sits npc is an open seat
    public void SitInSeat(NPC npc) {
        seats[0].SitDown(npc);
    }

    //Stands NPC up out of the seat they were in
    public void GetOutOfSeat(NPC npc) {
        seats[0].StandUp(npc);
    }

    //returns if inventory is empty
    public bool HasItems() {
        return !dynamicInventory.IsEmpty();
    }

    //Packs crates into the inventory and changes their names
    public void PackDriveable(Godot.Collections.Dictionary<ItemR, int> cratesToPack) {
        foreach (ItemR item in cratesToPack.Keys) {
            int amt = cratesToPack[item];
            CrateR crate = new CrateR(amt, crateScene, item.GetItemName);
            dynamicInventory.AddToInventory(crate, 1);
            //give it a name
            foreach (ItemR itemSpawned in dynamicInventory.GetItemObjects.Keys) {
                if (itemSpawned.Equals(item))
                    ChangeCrateNames(dynamicInventory.GetItemObjects[itemSpawned], item, amt, crate.GetItemName);
            }
        }
    }

    //removes a random item from the inventory
    public ItemInfo RemoveOneItem() {
        return dynamicInventory.RemoveRandomItemFromInventory();
    }

    //don't collide with other things and hopefully npcs
    public void OnNavigationAgent3DVelocityComputed(Vector3 safeVel) {
        Velocity = safeVel;
        MoveAndSlide();
    }

    //Updates the crate names to match what's inside them
    private void ChangeCrateNames(List<Item> items, ItemR itemR, int amt, string itemName) {
        foreach (Item item in items)
            if (item is Crate crate)
                crate.MakeCrate(itemName, amt, itemR);
    }
}

