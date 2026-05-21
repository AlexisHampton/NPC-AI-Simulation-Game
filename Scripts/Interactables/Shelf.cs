using Godot;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

[GlobalClass]
//Shelf holds items in an inventory
public partial class Shelf : StaticBody3D, IInteractable {

    [Export] private Item stockedItem;
    [Export] private int amtToSpawnDebug = 10;
    [Export] protected DynamicInventory dynamicInventory;
    [Export] private InventoryUIDynamicInventory inventoryUI;


    public ItemR GetStockedItem { get => stockedItem.ItemR; }

    public override void _Ready() {
        if (stockedItem != null)
            StockShelf(stockedItem, amtToSpawnDebug);
        inventoryUI.Visible = false;
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
    public void StockShelf(Item item, int amount) {
        dynamicInventory.AddToInventory(item, amount);
    }

    //adds specified itemR and amount to the inventory
    public void StockShelf(List<ItemInfo> items) {
        dynamicInventory.AddToInventory(items);
    }

    //removes an item from the inventory with a specified amount
    public void TakeItem(ItemR item, int amount) {
        dynamicInventory.RemoveFromInventory(item, amount);
    }

    //Removes a random item from inventory with a specified amount
    public InventoryCell TakeRandomItem(int amount) {
        if (IsEmpty()) return null;
        List<InventoryCell> allItems = dynamicInventory.GetItemsInInventory();
        int randNum = GD.RandRange(0, allItems.Count - 1);
        InventoryCell ic = new InventoryCell(allItems[randNum].item, amount);
        TakeItem(ic.itemR, amount);
        return ic;
    }

    public virtual void Interact(Node3D body) {
        if (body is not Player player) return;
        //if player has an item, add to inventory
        if (!player.AreHandsEmpty()) {
            Item item = player.PutDown(false);
            item.DisableName(true);
            item.DisableCollision(true);
            dynamicInventory.AddToInventory(item);
        } //otw show inv ui so player can take an item
        else {
            List<InventoryCell> cells = dynamicInventory.GetItemsInInventory();
            inventoryUI.LoadItemsIntoInvCells(cells);
            inventoryUI.SetUpInventory(player, dynamicInventory);
            inventoryUI.TurnInventoryOn(true);
        }

    }
}

