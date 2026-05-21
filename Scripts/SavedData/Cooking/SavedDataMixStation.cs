using Godot;
using Godot.Collections;

public partial class SavedDataMixStation : SavedDataPrepStation {
    [Export] public RecipieR currRecipie;
    [Export] public Array<string> spawnedIngredients = [];
    [Export] public Array<IngredientR> ingredientRs = [];
    [Export] public bool finalDishSpawned = false;
}
