using Godot;
using System;

//Choppable station chops ingredients and spawns a chopped ingredient
public partial class ChoppableStation : PrepStation {

    //Spawns a chopped ingredient when finished
    public override void ProcessIngredient() {
        base.ProcessIngredient();

        //if done, spawn choppable scene
        if (currStep >= maxStep && ingAdded.ChoppedScene != null) {
            SpawnNewIngredient(ingAdded.ChoppedScene);
            ResetProgressUI();
            return;
        }
    }


}
