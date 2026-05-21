using Godot;

//Station is the base class for all the Cooking stations
//It allows ingredients to be added and removed and to can return a random maxStep
[GlobalClass]
public abstract partial class Station : StaticBody3D, IAltInteractable, ISaveData {

    [Export] private MeshInstance3D mesh;
    [Export] protected Node3D ingredientSpawnPosition;

    protected Ingredient ingAdded;
    protected int maxStep;

    //Returns if there is an ingredient in this station
    public bool HasIngredient() => ingAdded is not null;

    //Adds the specified ingredient
    public virtual void AddIngredient(Ingredient ingredient) {
        if (HasIngredient()) return;
        ingAdded = ingredient;
        ingAdded.DisableCollision(true);
    }

    //Removes the ingredient on the station
    public virtual Ingredient RemoveIngredient() {
        Ingredient ingredientAgain = ingAdded;
        ingAdded = null;
        return ingredientAgain;
    }

    //Returns a random int from 3 to 10
    protected virtual int GetMaxSteps() => GD.RandRange(3, 10);

    //When alt interact key is pressed, the ingredient can be processed
    public virtual void ProcessIngredient() { }

    //Methods called for interaction
    public virtual void Interact(Node3D body) { }
    public virtual void AltInteract(Node3D body) { }

    //Loads the Ingredient on the counter
    public virtual void OnLoad(SavedData savedData) {
        SavedDataStation sd = (SavedDataStation)savedData;
        //if there is an item, delete it
        if (ingAdded is not null && IsInstanceValid(ingAdded)) {
            Ingredient ing = RemoveIngredient();
            ing.QueueFree();
        }
        //spawn the item
        if (sd.ingredientScene != "")
            ingAdded = (Ingredient)Utilities.InstantiateItem(sd.ingredientScene,
                ingredientSpawnPosition.Position,
                this);
    }

    //Saves the Ingredient on the counter
    public virtual SavedData OnSave() {
        SavedDataStation sd = new();
        SaveStationData(sd);
        return sd;
    }

    //Saves ingredient Scene
    public void SaveStationData(SavedDataStation sd) {
        sd.scenePath = SceneFilePath;
        if (ingAdded is not null)
            sd.ingredientScene = ingAdded.ItemR.ItemScenePath;
    }
}
