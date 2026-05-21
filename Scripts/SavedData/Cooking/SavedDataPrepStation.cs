using Godot;

//SavedDataPrepStation stores all the progress and progressUI data for a PrepStation
public partial class SavedDataPrepStation : SavedDataStation {
    [Export] public bool progressUIVisible;
    [Export] public float currStep;
    [Export] public int maxStep;
    [Export] public bool hasNewIngredientSpawned;
    [Export] public double currValue;
}
