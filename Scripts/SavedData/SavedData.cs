using Godot;
using System;

//SavedData is used to save data for dynamic objects like NPCs or Items
public partial class SavedData : Resource {
    [Export] public int Index = -1;
    [Export] public Vector3 position;
    [Export] public string scenePath;
}
