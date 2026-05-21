using Godot;

//PrepStation is an abstract class where ingredients can be processed
public abstract partial class PrepStation : CounterStation, ISaveData {
    //dish to spawn if the "process" i.e. chop, does not produce a result
    [Export] protected PackedScene badDish;

    [ExportGroup("Progress Bar")]
    [Export] protected Node3D progressUI;
    [Export] protected ProgressBar progressBar;

    protected float currStep = 0;
    protected bool hasNewIngredientSpawned = false;

    //At start, turn off progressUI
    public override void _Ready() {
        progressUI.Visible = false;
    }

    //When an ingredient is added, reset the progress UI
    public override void AddIngredient(Ingredient ingredient) {
        base.AddIngredient(ingredient);
        ResetProgressUI(true, ingredient.CurrProgress);
    }

    //When player hits the alternate interact key, process the ingredient and increase the progressBar
    public override void ProcessIngredient() {
        if (maxStep <= 0) {
            GD.PrintS("Cannot process", ingAdded.Name, "because max step is", maxStep);
            return;
        }

        currStep++;
        ingAdded.IncreaseCurrProgress(1);
        progressBar.Value = currStep / maxStep;
    }

    //when player removes an ingredient, turn the progress UI off
    public override Ingredient RemoveIngredient() {
        ResetProgressUI();
        return base.RemoveIngredient();
    }

    //Reset the progress bar and turn on/off the ui
    protected void ResetProgressUI(bool isOn = false, float step = 0) {
        currStep = step;
        if (step == 0) {
            maxStep = GetMaxSteps();
            progressBar.Value = step / maxStep;
        }
        progressUI.Visible = isOn;
    }

    //Spawns a new ingredient and puts it on the counter
    protected Ingredient SpawnNewIngredient(PackedScene ingToSpawn) {
        Ingredient newIng = (Ingredient)Utilities.InstantiateItem(ingToSpawn,
        Vector3.Zero, this);
        newIng.DisableCollision(true);

        //delete old ingredient 
        ingAdded.QueueFree();

        ingAdded = newIng;
        newIng.GlobalPosition = ingredientSpawnPosition.GlobalPosition;

        return newIng;
    }

    //On alt interact, process the ingredient
    public override void AltInteract(Node3D body) {
        if (HasIngredient())
            ProcessIngredient();
        base.AltInteract(body);
    }

    //Loads the progress UI data and progress variables
    public override void OnLoad(SavedData savedData) {
        base.OnLoad(savedData);
        SavedDataPrepStation sdp = (SavedDataPrepStation)savedData;
        currStep = sdp.currStep;
        maxStep = sdp.maxStep;
        hasNewIngredientSpawned = sdp.hasNewIngredientSpawned;
        progressBar.Value = sdp.currValue;

        progressUI.Visible = sdp.progressUIVisible;
    }

    //Saves the progress UI data and progress variables
    public override SavedData OnSave() {
        SavedDataPrepStation sdp = new();
        SavePrepData(sdp);
        return sdp;
    }

    //Saves all PrepStation and Station variables
    public void SavePrepData(SavedDataPrepStation sdp) {
        SaveStationData(sdp);
        sdp.progressUIVisible = progressUI.Visible;
        sdp.currStep = currStep;
        sdp.maxStep = maxStep;
        sdp.hasNewIngredientSpawned = hasNewIngredientSpawned;
        sdp.currValue = progressBar.Value;
    }

}
