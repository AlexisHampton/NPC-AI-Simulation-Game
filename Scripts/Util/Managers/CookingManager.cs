using Godot;
using Godot.Collections;
using System.Collections.Generic;

//CookingManager holds all the cooking information
public partial class CookingManager : Node {

    [Export] public Array<RecipieR> AllRecipies { get; private set; }

}
