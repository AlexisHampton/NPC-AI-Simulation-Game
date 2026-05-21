using Godot;
using System.Collections.Generic;
using Godot.Collections;

//MixStation is a PrepStation that when processed spawns a new food from a list of available recipies
public abstract partial class MixStation : PrepStation {
    [Export] private Node3D appliance; //???

    protected RecipieR currRecipie; //the closest matching recipie 
    protected Array<IngredientR> ingredientRs = []; //only an array to make it easier to save
    protected List<Ingredient> spawnedIngredients = [];

    private bool finalDishSpawned = false;
    private bool hasAddedSeasoning = false;

    //Adds the ingredientRs for all the ingredients on the Station
    public override void AddIngredient(Ingredient ingredient) {
        //if player adds seasoning, tick the flag and destroy it
        if (ingredient.ItemR.ItemType == ItemType.SEASONING) {
            hasAddedSeasoning = true;
            ingredient.QueueFree();
            return;
        }
        //add the ingredient 
        spawnedIngredients.Add(ingredient);
        ingredientRs.Add((IngredientR)ingredient.ItemR);
        base.AddIngredient(ingredient);
    }

    //Process the ingredient and spawn the finished recipie
    public override void ProcessIngredient() {
        base.ProcessIngredient();
        //if done, spawn finished recipie
        FinishIngredient();
    }

    //Spawn the finished ingredient and reset currStep so that interaction doesn't process ingredients that don't exist
    protected void FinishIngredient() {
        if (currStep < maxStep) return;
        currStep = -10;
        SpawnFinalDish();
        ResetProgressUI();
    }

    //When ingredient is finished being processed, get rid of all ingredients
    public override Ingredient RemoveIngredient() {
        finalDishSpawned = false;
        hasAddedSeasoning = false;
        spawnedIngredients.Clear();
        return base.RemoveIngredient();
    }

    //Returns the recipieR that has all the ingredients 
    protected RecipieR FindBestRecipie() {
        foreach (RecipieR recipieR in Globals.Instance.CookingManager.AllRecipies)
            if (CanSpawnDish(recipieR) && recipieR.HasAllIngredients(ingredientRs))
                return recipieR;
        return null;
    }

    //Returns true if the ingredients' recipieSteps match the recipieSteps in the recipie
    protected bool CheckRecipieSteps(RecipieR recipieR) {
        foreach (Ingredient ing in spawnedIngredients) {
            if (!recipieR.MatchesRecipieStep(ing)) return false;
        }
        return true;
    }

    //Spawns the final dish and despawns the ingredients
    protected void SpawnFinalDish() {
        currRecipie = FindBestRecipie();
        bool hasDoneAllSteps = CheckRecipieSteps(currRecipie);

        int score = GetScore(hasDoneAllSteps);

        DespawnIngredients();

        PackedScene finalDish = badDish;
        if (currRecipie is not null && CanSpawnDish(currRecipie)) {
            finalDish = currRecipie.FinalDishScene;
        }
        Ingredient spawnedIng = SpawnNewIngredient(finalDish);
        spawnedIng.ItemR.UpgradeAllStats(score);
        GD.PrintS("MixStation:", spawnedIng.GetIngredientR().ItemName, spawnedIng.ItemR.PrintStats(" "));
        spawnedIngredients.Add(spawnedIng);
        finalDishSpawned = true;
    }

    //Returns a score for the dish
    private int GetScore(bool hasDoneAllSteps) {
        int score = 1; // completion score
        score = hasAddedSeasoning ? score + 1 : score; // +1 for seasoning
        score = hasDoneAllSteps ? score + 1 : score; // +1 for following directions
        return score;
    }

    //Checks if this station can carry out that recipie
    protected virtual bool CanSpawnDish(RecipieR recipie) => recipie.IsMixRecipie;

    //Despawns all the ingredients
    protected void DespawnIngredients() {
        foreach (Ingredient ing in spawnedIngredients)
            ing.QueueFree();
        spawnedIngredients.Clear();
        ingredientRs.Clear();
    }

    //Accepts ingredients even if other ingredients are on the station
    protected override bool CanAcceptIngredient(Player player) {
        return !player.AreHandsEmpty() && player.GetItem() is Ingredient;
    }

    //Removes the ingredient only if the final dish has been spawned
    protected override bool CanRemoveIngredient(Player player) {
        return player.AreHandsEmpty() && finalDishSpawned;
    }

    //Saves the spawned ingredients and current recipie data
    public override SavedData OnSave() {
        SavedDataMixStation sd = new();
        SavePrepData(sd);
        sd.currRecipie = currRecipie;
        sd.ingredientRs = ingredientRs;
        sd.finalDishSpawned = finalDishSpawned;

        //Save spawned ingredients
        Array<string> spawnedIngs = [];
        foreach (Ingredient ing in spawnedIngredients) {
            spawnedIngs.Add(ing.ItemR.ItemScenePath);
        }
        sd.spawnedIngredients = spawnedIngs;

        return sd;
    }

    //Loads the spawned ingredients and current recipie data
    public override void OnLoad(SavedData savedData) {
        SavedDataMixStation sd = (SavedDataMixStation)savedData;
        base.OnLoad(sd);
        DespawnIngredients();

        currRecipie = sd.currRecipie;
        ingredientRs = sd.ingredientRs;
        finalDishSpawned = sd.finalDishSpawned;

        //despawn any ingredients currently being used
        //repopulate spawnedIngredients
        foreach (string ingPath in sd.spawnedIngredients) {
            Ingredient ing = (Ingredient)Utilities.InstantiateItem(
                ingPath,
                Vector3.Zero,
                this
            );
            ing.GlobalPosition = ingredientSpawnPosition.GlobalPosition;
            spawnedIngredients.Add(ing);
        }
    }

}
