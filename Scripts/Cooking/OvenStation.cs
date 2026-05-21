using Godot;
using System;

//OvenStation is a MixStation that 
public partial class OvenStation : MixStation {

    //Adds an ingredient to the station
    public override void AddIngredient(Ingredient ingredient) {
        //if first ingredient is butter, increase score by 50
        base.AddIngredient(ingredient);
    }

    //Every frame, increases currStep by the time between frames until its finished
    public override void _Process(double delta) {
        if (HasIngredient()) {
            currStep += (float)delta * 3;
            IncreaseProgress((float)delta * 3);
        }
    }

    //Update progressbar and save current progress to ingredient
    private void IncreaseProgress(float amt) {
        progressBar.Value = currStep / maxStep;
        ingAdded.IncreaseCurrProgress(amt);
        FinishIngredient();
    }

    //When "stirring" the pot, increase the currStep by a lot
    public override void ProcessIngredient() {
        base.ProcessIngredient();

        currStep += 10f;
        IncreaseProgress(10);
    }

    //Dish can only be spawned if it's an heat recipie
    protected override bool CanSpawnDish(RecipieR recipie) {
        return !recipie.IsMixRecipie;
    }

    //Returns a random value from a bigger range 
    protected override int GetMaxSteps() => GD.RandRange(50, 120);


}
