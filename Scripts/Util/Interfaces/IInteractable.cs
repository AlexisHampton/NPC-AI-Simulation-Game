using Godot;
using System;

//IInteractable is implemented on all objects that the player can interact with
public interface IInteractable {
    public void Interact(Node3D body);
}
