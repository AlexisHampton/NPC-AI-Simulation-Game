using Godot;
using System;
using System.Diagnostics.Contracts;

//Item is the instantiated form of ItemR, it holds location, mesh, and collision information
public partial class Item : Node3D, IInteractable {

    [Export] public ItemR ItemR { get; private set; }
    [Export] private CollisionShape3D collisionShape;

    //Turns the mesh and collision on or off
    public void DisableItem(bool canDisable) {
        collisionShape.Disabled = canDisable;
        Visible = !canDisable;
    }

    //Adds the item to the player's inventory
    public void Interact(Node3D body) {
        if (body is Player player) {
            player.Inventory.AddToInventory(this, 1);
            DisableItem(true);
        }
    }

    //equality is only within the itemR
    public override bool Equals(object obj) {
        if (obj is not Item other) return false;
        if (other.ItemR != ItemR) return false;
        return true;
    }

    //Returns the hash code based on the ItemR
    public override int GetHashCode() {
        return HashCode.Combine(ItemR);
    }
}
