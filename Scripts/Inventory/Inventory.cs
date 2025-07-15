using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[GlobalClass]

//Inventory is the base class for Dynamic Inventory, Basket, and Hands
public partial class Inventory : Node3D {

    protected Dictionary<ItemR, InventoryCell> inventoryCells = new Dictionary<ItemR, InventoryCell>();

    protected bool isFull;

    //Check if inventory is empty
    public bool IsEmpty() {
        return inventoryCells.Count == 0;
    }

    //returns if inventory is full or not
    public virtual bool IsFull() {
        return isFull;
    }

    //check for item in inventory
    public bool HasItem(ItemR item) {
        if (item is not null && inventoryCells.ContainsKey(item))
            return true;
        return false;
    }

    //Returns the amount of a specified itemR
    public int GetItemAmount(ItemR item) {
        if (!inventoryCells.TryGetValue(item, out InventoryCell itemFound)) return 0;
        return itemFound.itemAmount;
    }

    //Gets all items in the inventory as a list
    public List<ItemR> GetItemsInInventory() {
        return [.. inventoryCells.Keys];
    }

    //Returns a list of all the items with a specified itemType
    public List<InventoryCell> GetAllItemsByType(ItemType type) {
        List<InventoryCell> ics = new();

        foreach (ItemR itemR in inventoryCells.Keys)
            if (itemR.ItemType == type)
                ics.Add(inventoryCells[itemR]);
        return ics;
    }

    //Add item to inventory
    public virtual void AddToInventory(Item item, int amt) {
        //check if in inventory and update amount
        if (HasItem(item.ItemR))
            inventoryCells[item.ItemR].itemAmount += amt;
        else { //make a new one
            InventoryCell ic = new InventoryCell(item, amt);
            inventoryCells.Add(item.ItemR, ic);
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
    public virtual InventoryCell RemoveFromInventory(ItemR item, int amt) {
        if (!HasItem(item))
            return null;
        InventoryCell ic = inventoryCells[item];
        inventoryCells[item].itemAmount -= amt;
        if (inventoryCells[item].itemAmount <= 0)
            inventoryCells.Remove(item);
        return ic;

    }

    // //remove all items from inventory
    // public virtual List<InventoryCell> EmptyInventory() {
    //     if (IsEmpty()) return null;
    //     List<ItemR> items = GetItemsInInventory();
    //     List<InventoryCell> removedItems = new List<InventoryCell>();
    //     foreach (ItemR item in items) {
    //         int numItems = inventoryCells[item].itemAmount;
    //         InventoryCell ic = new InventoryCell(item, numItems);
    //         removedItems.Add(ic);
    //     }

    //     inventoryCells.Clear();
    //     return removedItems;
    // }

}