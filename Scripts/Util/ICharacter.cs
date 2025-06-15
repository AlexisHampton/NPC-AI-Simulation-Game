using Godot;
using System;

//ICharacter hold all methods players,npcs, and any other character classes might have in common in lieu of an actual character class
public partial interface ICharacter {
    public void PickUp(IGatherable gatherable);
    public IGatherable PutDown();
}
