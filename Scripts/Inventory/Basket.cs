using Godot;
using System;
using System.Collections.Generic;

//Basket is a small Dynamic Inventory that NPCs or players can hold 
public partial class Basket : DynamicInventory {

    [Export] private Node3D basket;

    //Adds an amount of item to the basket
    public void AddToBasket(Item item, int amt) {
        basket.Visible = true;
        AddToInventory(item, amt);
    }

    //Adds an already spawned item to the basket
    public void AddToBasket(Item item) {
        basket.Visible = true;
        AddToInventory(item);
    }

    //Removes an amount of the specified itemR from the basket
    public void RemoveFromBasket(ItemR item, int amt) {
        RemoveFromInventory(item, amt);
        if (IsEmpty())
            basket.Visible = false;
    }

    //Removes all the items from the basket
    public List<ItemInfo> EmptyBasket() {
        List<ItemInfo> itemInfos = RemoveAllFromInventory();
        basket.Visible = false;
        return itemInfos;
    }

}
