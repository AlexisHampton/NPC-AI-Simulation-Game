using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;

public struct OrderInfo {
    public NPC customer;
    public ItemR order;

    public OrderInfo(NPC npc, ItemR orderIn) {
        customer = npc;
        order = orderIn;
    }
}

[GlobalClass]

//Tavern is a Place with JobTasks 
public partial class Tavern : CommercialSpace {

    [Export] private DynamicInventory foodStock;
    private List<OrderInfo> orderInfos = new List<OrderInfo>();
    private List<OrderInfo> completedOrders = new List<OrderInfo>();


    public override void _Ready() {
        base._Ready();
        foreach (Task task in placeTasks)
            if (task is TavernTask tt)
                tt.SetStore(this);
    }
    //Sets store tasks tavern. 
    public override void AddTask(Task task) {
        base.AddTask(task);
        if (task is TavernTask tt)
            tt.SetStore(this);
    }
    //-------------- Food Counter --------------
    public bool IsFoodStockFull() {
        return foodStock.IsFull();
    }

    public void AddToFoodStock(OrderInfo currOrder, Item item) {
        completedOrders.Add(currOrder);
        foodStock.AddToInventory(currOrder.order, item);
    }

    public Item RemoveFromFoodStock(ItemR item) {

        return foodStock.RemoveFromInventory(item).item;
    }

    public Vector3 GetFoodStockPositon() {
        return foodStock.GlobalPosition;
    }

    //--------------Orders------------

    public bool HasOrders() {
        return orderInfos.Count != 0;
    }

    public bool HasCompletedOrders() {
        return completedOrders.Count != 0;
    }

    public void AddOrder(NPC npc) {
        OrderInfo orderInfo = new OrderInfo(npc, GetRandomOrder());
        orderInfos.Add(orderInfo);

    }

    public OrderInfo RemoveFirstOrder() {
        OrderInfo orderInfo = orderInfos[0];
        orderInfos.Remove(orderInfo);
        return orderInfo;
    }

    public OrderInfo RemoveFirstCompletedOrder() {
        OrderInfo orderInfo = completedOrders[0];
        completedOrders.Remove(orderInfo);
        return orderInfo;
    }
    public ItemR GetRandomOrder() {
        int randNum = GD.RandRange(0, itemsToSell.Count - 1);
        return itemsToSell[randNum];
    }



    //removes a customer from the line
    public NPC RemoveCustomerFromLine() {
        NPC npc = line[0];
        line.RemoveAt(0);
        return npc;
    }


}
