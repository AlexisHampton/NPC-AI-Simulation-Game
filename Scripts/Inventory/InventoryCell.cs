using Godot;
using System;
[GlobalClass]

//holds an item and itemAmount for a single cell of an inventory
public partial class InventoryCell : Node {

    public Item item;
    public ItemR itemR;
    public int itemAmount;

    //Creates an empty inventory cell
    public InventoryCell() {
        item = null;
        itemR = null;
        itemAmount = 0;
    }

    //Creates an inventory cell with an item and the amount
    public InventoryCell(Item itemIn, int amtIn) {
        item = itemIn;
        itemR = itemIn.ItemR;
        itemAmount = amtIn;
    }
}
