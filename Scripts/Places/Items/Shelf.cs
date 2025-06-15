using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
//Shelf holds items in an inventory
public partial class Shelf : StaticBody3D {

    [Export] private ItemR stockedItem;
    [Export] private int amtToSpawnDebug = 10;
    [Export] private DynamicInventory dynamicInventory;

    public ItemR GetStockedItem { get => stockedItem; }

    public override void _Ready() {
        if (stockedItem != null)
            StockShelf(stockedItem, amtToSpawnDebug);
    }

    //Returns if inventory is full
    public bool IsFull() {
        return dynamicInventory.IsFull();
    }

    //Returns if inventory is empty
    public bool IsEmpty() {
        return dynamicInventory.IsEmpty();
    }

    //Returns if inventory has specified item
    public bool HasItems(ItemR item) {
        return dynamicInventory.HasItem(item);
    }

    //adds specified itemR and amount to the inventory
    public void StockShelf(ItemR item, int amount) {
        dynamicInventory.AddToInventory(item, amount);
    }

    //adds a crate's contents to the inventory
    public void StockShelf(Crate crate) {
        dynamicInventory.AddToInventory(crate.GetItemR, crate.GetItemAmount);
    }

    //adds a crate's contents to the inventory
    public void StockShelf(List<ItemInfo> itemInfos) {
        foreach (ItemInfo itemInfo in itemInfos) {
            dynamicInventory.AddToInventory(itemInfo.itemR, itemInfo.item);
        }
    }

    //removes an item from the inventory with a specified amount
    public void TakeItem(ItemR item, int amount) {
        dynamicInventory.RemoveFromInventory(item, amount);
    }

    //Removes a random item from inventory with a specified amount
    public InventoryCell TakeRandomItem(int amount) {
        if (IsEmpty()) return null;
        List<ItemR> allItems = dynamicInventory.GetItemsInInventory();
        int randNum = GD.RandRange(0, allItems.Count - 1);
        InventoryCell ic = new InventoryCell(allItems[randNum], amount);
        TakeItem(ic.item, amount);
        return ic;
    }

}

