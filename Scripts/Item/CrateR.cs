using Godot;
using System;

[GlobalClass]

//CrateR holds a set number of itemRs
public partial class CrateR : ItemR, IGatherable {

    [Export] private int itemAmount;

    public int GetItemAmount { get => itemAmount; }

    public CrateR(int amt, PackedScene crateScene, string crateName) {
        itemAmount = amt;
        itemScene = crateScene;
        itemName = crateName;
    }

    public override bool Equals(object obj) {
        //if (obj is not CrateR) return false;
        //CrateR other = (CrateR)obj;
        //if (itemAmount != other.itemAmount) return false;
        return base.Equals(obj);
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }
}

