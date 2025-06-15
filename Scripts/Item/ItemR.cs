using Godot;
using System;

[GlobalClass]

//ItemR holds an item scene to be instantiated, and is usually stored in inventories
public partial class ItemR : Resource, IGatherable {

    [Export] protected PackedScene itemScene;
    [Export] protected string itemName;

    public string GetItemName { get => itemName; }

    public PackedScene GetPackedScene() {
        return itemScene;
    }

    public override bool Equals(object obj) {
        if (obj is not ItemR) return false;
        ItemR other = (ItemR)obj;
        if (itemName != other.itemName) return false;
        return true;
    }

    public override string ToString() {
        return itemName;
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }
}
