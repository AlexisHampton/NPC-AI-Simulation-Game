using Godot;
using System;

//Not implemented yet, just stolen from a previous project of mine, but is not used currently.
public partial class BusinessDoor : Area3D {

    [Export] private Store store;

    public void OnBodyEntered(Node3D body) {
        if (body is NPC npc && store.IsNPCOwner(npc))
            store.OpenStore(true);
    }

    public void OnBodyExited(Node3D body) {
        if (body is NPC npc && store.IsNPCOwner(npc))
            store.OpenStore(false);
    }
}
