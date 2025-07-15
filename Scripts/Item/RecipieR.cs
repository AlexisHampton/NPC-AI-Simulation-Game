using Godot;
using Godot.Collections;

[GlobalClass]
//RecipieR holds the ingredient list and final item of a recipie
public partial class RecipieR : Resource {

    [Export] public string RecipieName { get; private set; }
    [Export(PropertyHint.MultilineText)] public string Description { get; private set; }
    [Export] public Dictionary<IngredientR, int> Ingredients { get; private set; } = new();
    [Export] public PackedScene completedItem { get; set; }


}
