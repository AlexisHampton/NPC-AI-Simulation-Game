using Godot;
using System;

//Hands is a single-celled inventory whose item/itemR spawns in the game world
public partial class Hands : Node3D {

    //[Export] private Node3D handsPos;

    private ItemR itemR = null;
    private Item item = null;
    private Node3D itemSpawned;

    //returns whether or not hands is empty
    public bool IsEmpty() {
        return itemR == null && item == null;
    }

    //Adds the specified ItemR to inventory and spawns it in the world
    public void PickUp(ItemR itemIn) {
        if (!IsEmpty()) return;
        itemR = itemIn;
        //instantiate
        itemSpawned = (Node3D)itemR.GetPackedScene().Instantiate();
        itemSpawned.Position = Position;
        AddChild(itemSpawned);
    }

    //Adds the specified item to inventory and updates its position
    public void PickUp(Item itemIn) {
        if (!IsEmpty()) return;
        item = itemIn;
        item.Reparent(this);
        item.Position = Position;
        item.Visible = true;
    }

    //Gets rid of an item from the inventory and destroys it or returns it
    public Item PutDown(bool canDestroyItem) {
        if (IsEmpty()) return null;
        if (canDestroyItem) {
            item.QueueFree();
            item = null;
            return null;
        }

        Item i = item;
        item = null;
        return i;
    }


}
