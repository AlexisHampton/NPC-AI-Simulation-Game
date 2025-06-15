using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]

//Inventory is the base class for Dynamic Inventory, Basket, and Hands
public partial class Inventory : Node3D {

    //protected List<InventoryCell> inventoryCells = new List<InventoryCell>();
    protected Dictionary<ItemR, InventoryCell> inventoryCells = new Dictionary<ItemR, InventoryCell>();

    protected bool isFull;

    //Check if inventory is empty
    public bool IsEmpty() {
        return inventoryCells.Count == 0;
    }

    public virtual bool IsFull() {
        return isFull;
    }

    //check for item in inventory
    public bool HasItem(ItemR item) {
        if (item is not null && inventoryCells.ContainsKey(item))
            return true;
        return false;
    }

    //Gets all items in the inventory as a list
    public List<ItemR> GetItemsInInventory() {
        List<ItemR> items = new List<ItemR>();
        foreach (ItemR key in inventoryCells.Keys)
            items.Add(key);
        return items;
    }

    //Add item to inventory
    public virtual void AddToInventory(ItemR item, int amt) {
        //check if in inventory and update amount
        if (HasItem(item))
            inventoryCells[item].itemAmount += amt;
        else { //make a new one
            InventoryCell ic = new InventoryCell(item, amt);
            inventoryCells.Add(item, ic);
        }
    }
    /*
    //remove an item from inventory with a specified amount
    public virtual void RemoveFromInventory(ItemR item, int amt) {
        if (!HasItem(item))
            return;

        inventoryCells[item].itemAmount -= amt;
        if (inventoryCells[item].itemAmount <= 0)
            inventoryCells.Remove(item);
    }
    */

    //remove an item from inventory with a specified amount
    public virtual void RemoveFromInventory(ItemR item, int amt) {
        if (!HasItem(item))
            return;

        inventoryCells[item].itemAmount -= amt;
        if (inventoryCells[item].itemAmount <= 0)
            inventoryCells.Remove(item);
    }

    //remove all items from inventory
    public virtual List<InventoryCell> EmptyInventory() {
        if (IsEmpty()) return null;
        List<ItemR> items = GetItemsInInventory();
        List<InventoryCell> removedItems = new List<InventoryCell>();
        foreach (ItemR item in items) {
            int numItems = inventoryCells[item].itemAmount;
            InventoryCell ic = new InventoryCell(item, numItems);
            removedItems.Add(ic);
        }

        inventoryCells.Clear();
        return removedItems;
    }

}