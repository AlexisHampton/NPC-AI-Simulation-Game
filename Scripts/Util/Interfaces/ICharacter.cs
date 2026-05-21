using Godot;
using System;
using System.Collections.Generic;

//ICharacter hold all methods players,npcs, and any other character classes might have in common in lieu of an actual character class
public partial interface ICharacter {
    //Hands methods
    public void PickUp(Item item);
    public Item PutDown(bool canDestroyItem);
    public bool AreHandsEmpty();
    public Item GetItem();

    //---Basket---
    //Checks if the basket is empty
    public bool IsBasketEmpty();
    public bool IsBasketFull();
    public void AddToBasket(Item item, int amt);
    public void AddToBasket(Item item);
    public void RemoveFromBasket(ItemR item, int amt);
    public List<ItemInfo> EmptyBasket();
    public Vector3 GetGlobalPosition();
}
