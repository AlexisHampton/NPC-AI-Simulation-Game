using Godot;
using System;

//BowlStation mixes ingredients in what will possibly be a Bowl in the future
public partial class BowlStation : MixStation {

    //Process ingredients only if there are more than two of them
    public override void AltInteract(Node3D body) {
        if (ingredientRs.Count >= 2)
            ProcessIngredient();
    }
}
