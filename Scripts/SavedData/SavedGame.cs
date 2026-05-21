using Godot;
using Godot.Collections;
using System;

//SavedGame contains all top level data that will be saved
//It also contains a list of all the dynamic data that needs to be saved
public partial class SavedGame : Resource {
    //static elements
    [Export] public SavedDataPlayer playerData;
    [Export] public SavedData gameTimeData;

    //dynamic elements
    [Export] public Array<SavedData> savedData = new Array<SavedData>();
}
