using Godot;
using System;
using System.Dynamic;

[GlobalClass]

//ItemR holds an item scene to be instantiated, and is usually stored in inventories
public partial class ItemR : Resource {

    [Export] public string ItemName { get; private set; }
    [Export] public ItemType ItemType { get; private set; }
    [Export(PropertyHint.MultilineText)] public string ItemDescr { get; private set; }
    [Export] public Texture2D Sprite { get; private set; }

    //checks if all fields are equal
    public override bool Equals(object obj) {
        if (obj is not ItemR) return false;
        ItemR other = (ItemR)obj;
        if (ItemName != other.ItemName) return false;
        if (ItemDescr != other.ItemDescr) return false;
        if (Sprite != other.Sprite) return false;
        if (ItemType != other.ItemType) return false;
        return true;
    }

    //returns itemName
    public override string ToString() {
        return ItemName;
    }

    //returns a hash code based on the name, sprite, type, and description
    public override int GetHashCode() {
        return HashCode.Combine(ItemName, Sprite, ItemType, ItemDescr);
    }
}
