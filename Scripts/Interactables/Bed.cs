using Godot;
using System;

//Bed is an item both the player and npcs can interact with. 
//Npcs can do a sleepTask on it
//The player can advance to the next day on it
public partial class Bed : Sittable, IInteractable {

    //Player advances to the next day
    public void Interact(Node3D body) {
        Globals.Instance.GameTime.NextDay();
    }
}
