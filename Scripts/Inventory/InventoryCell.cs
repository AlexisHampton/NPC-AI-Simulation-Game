using Godot;
using System;
[GlobalClass]

//holds an item and itemAmount for a single cell of an inventory
public partial class InventoryCell : Node {

    public ItemR item;
    public int itemAmount;

    public InventoryCell() {
        item = null;
        itemAmount = 0;
    }

    public InventoryCell(ItemR itemIn, int amtIn) {
        item = itemIn;
        itemAmount = amtIn;
    }
}
