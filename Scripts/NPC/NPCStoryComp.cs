using System.Linq;
using Godot;
using Godot.Collections;

//NPCStoryComp allows NPCs to behave in a more story RPG-oriented manner
public partial class NPCStoryComp : Node {

    [Export] private Array<string> startSymbols;

    //whether npc has talked to player
    public bool HasTalked { get; private set; } = false;

    //Returns a random symbol for now
    public string GetRandSymbol() {
        HasTalked = true;
        return Utilities.GetRandSymbol(startSymbols);
    }


}
