using Godot;
using System;

//Counter Station holds an Ingredient on top of it
public partial class CounterStation : Station, ISaveData {

    //Reposition the Ingredient to be on top of the counter
    public override void AddIngredient(Ingredient ingredient) {
        base.AddIngredient(ingredient);
        //spawn ingredient in the world
        ingredient.Reparent(this);
        ingredient.GlobalPosition = ingredientSpawnPosition.GlobalPosition;
    }

    //Returns if an ingredient can be processed
    protected virtual bool CanAcceptIngredient(Player player) {
        return !player.AreHandsEmpty() && !HasIngredient() && player.GetItem() is Ingredient;
    }

    //Returns if an ingredient can be removed
    protected virtual bool CanRemoveIngredient(Player player) {
        return player.AreHandsEmpty() && HasIngredient();
    }

    //Player can pick up or put down an item on the counter
    public override void Interact(Node3D body) {
        if (body is Player player) {
            if (CanRemoveIngredient(player))
                player.PickUp(RemoveIngredient());
            else if (CanAcceptIngredient(player))
                AddIngredient((Ingredient)player.PutDown(false));
        }
        base.Interact(body);
    }


}
