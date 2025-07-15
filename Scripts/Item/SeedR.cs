using Godot;

[GlobalClass]
//SeedR holds the days needed to grow and the final crop once it does
public partial class SeedR : ItemR {
    [Export] public int DaysUntilHarvest { get; private set; }
    [Export] public PackedScene FinalCrop { get; private set; }
}
