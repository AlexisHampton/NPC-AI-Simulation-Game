using Godot;
using System;

// An interface for objects that can be gathered by the Player, NPCs, or other character classes
public partial interface IGatherable {
    public PackedScene GetPackedScene();
}
