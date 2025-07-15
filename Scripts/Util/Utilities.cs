using Godot;
using System;

//Utilities holds all static methods that may be duplicated across files
public static partial class Utilities {

    //Adds an item to the player's inventory
    public static void AddItemToPlayerInventory(Player player, Item item) {
        item.DisableItem(true);

        //sets parent
        if (item.GetParent() is null)
            player.Inventory.AddChild(item);
        else
            item.Reparent(player.Inventory);

        item.Position = Vector3.Zero;
        player.Inventory.AddToInventory(item, 1);
    }

    //Instantiates an item and returns it
    public static Item InstantiateItem(PackedScene scene, Vector3 relativePos, Node3D parent) {
        Item newItem = scene.Instantiate<Item>();
        newItem.Position = relativePos;
        parent.AddChild(newItem);
        return newItem;
    }
}
