using Godot;
using System;

//Basket spawns ingredients 
public partial class IngredientSpawner : StaticBody3D, IInteractable {

    [Export] private Ingredient ingredientToSpawn;
    [Export] Label3D nameLabel;

    public override void _Ready() {
        nameLabel.Text = ingredientToSpawn.ItemR.ItemName;
        ingredientToSpawn.DisableItem(true);
    }

    //Returns a duplicate of the ingredient the basket spawns
    public Ingredient SpawnIngredient() {
        return (Ingredient)Utilities.InstantiateItem(ingredientToSpawn, Vector3.Zero, this);
    }

    //Player picks up a spawned ingredient
    public void Interact(Node3D body) {
        if (body is Player player) {
            Ingredient ing = SpawnIngredient();
            ing.DisableItem(false);
            player.PickUp(ing);
        }

    }

}
