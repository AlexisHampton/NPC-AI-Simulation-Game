using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;

[GlobalClass]

//Store is a Place with JobTasks 
public partial class Store : CommercialSpace {

    [ExportGroup("Cashier")]
    [Export] private CustomerCheckoutTask customerCheckoutTask;
    [Export] private Node3D custSpawnPos;
    [Export] private float custSpawnRadius;

    [ExportGroup("Delivery")]
    [Export] private Node3D traderDeliveryPoint;
    [Export] private Node3D cratesToStock;// dep
    [Export] private DynamicInventory stock;

    [ExportGroup("Stocker")]
    [Export] private Array<Shelf> shelves = new();


    //cashier stuff
    private Vector3 originalCustSpawnPosition;

    public CustomerCheckoutTask GetCustomerCheckoutTask { get => customerCheckoutTask; }
    //delivery stuff
    public Node3D GetTraderDeliveryPoint { get => traderDeliveryPoint; }

    public override void _Ready() {
        base._Ready();
        if (custSpawnPos != null)
            originalCustSpawnPosition = custSpawnPos.GlobalPosition;
        foreach (Task task in placeTasks)
            if (task is StoreTask tt)
                tt.SetStore(this);
    }

    //Sets store tasks store. 
    public override void AddTask(Task task) {
        base.AddTask(task);
        if (task is StoreTask st)
            st.SetStore(this);
    }


    //Returns a browseshelf task if an npc can do it
    public BrowseShelfTask GetBrowseTask(NPC npc) {
        foreach (Task task in placeTasks) {
            if (task is BrowseShelfTask bst && bst.CheckIfCanDoTask(npc))
                return bst;
        }

        return null;
    }


    //---------- Stock ----------
    public bool HasStock() {
        return !stock.IsEmpty();
    }

    public void AddToStock(ItemR itemR, Item item) {
        stock.AddToInventory(itemR, item);
    }

    //Removes a random item from stock if the specified item is null or not in stock
    //if the item exists and is in stock, removes that item from stock
    public ItemInfo RemoveFromStock(ItemR itemToRemove) {
        if (itemToRemove is null)
            return stock.RemoveRandomItemFromInventory();
        if (stock.HasItem(itemToRemove))
            return stock.RemoveFromInventory(itemToRemove);
        return stock.RemoveRandomItemFromInventory();
    }

    // ------ Shelves -----
    //Get an empty shelf
    public Shelf GetAnEmptyShelf() {
        foreach (Shelf shelf in shelves)
            if (shelf.IsEmpty())
                return shelf;
        return null;
    }

    //--------------Cashier------------

    //Gets the place for an npc to stand in line
    public Vector3 GetCustSpawnOffset() {
        custSpawnPos.Position += new Vector3(0f, 0f, -custSpawnRadius);
        return custSpawnPos.GlobalPosition;
    }

    //removes a customer from theline and moves everyone else up
    public void RemoveCustomerFromLine() {
        line.RemoveAt(0);
        MoveCustomersUpInLine();
        //decrease spawnPos
        custSpawnPos.Position -= new Vector3(0f, 0f, -custSpawnRadius);
        GD.PrintS("Cust spawn pos", custSpawnPos.Position);
    }

    //Each npc in the line, moves to the new offset position
    private void MoveCustomersUpInLine() {
        for (int i = 0; i < line.Count; i++) {
            line[i].Move(originalCustSpawnPosition + new Vector3(-custSpawnRadius * i, 0f, 0f));
        }
    }

    //--------------Delivery--------------------
    //deprecated
    public void SpawnCratesToStock(bool canSpawnCrates) {
        cratesToStock.Visible = canSpawnCrates;
    }
}
