using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

//Ingredient is an Item that can be used in the cooking system
[GlobalClass]
public partial class Ingredient : Item {

    [Export] public PackedScene ChoppedScene { get; private set; }
    [Export] public float CurrProgress { get; private set; }
    [Export] public StepType RecipieStep { get; private set; }

    //Returns the ingredientR
    public IngredientR GetIngredientR() => (IngredientR)ItemR;

    //Returns the max steps of the ingredientR
    public float GetMaxSteps() {
        if (ItemR is not IngredientR ingR)
            return 0;
        return ingR.MaxSteps;
    }
    //In case player takes an unfinished ing out of OvenStation or ChoppingStation
    public void IncreaseCurrProgress(float amt) {
        CurrProgress += amt;
        //what if go over
    }

}
