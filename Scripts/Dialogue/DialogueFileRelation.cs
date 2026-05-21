using Godot;

//Stores the dialogue file and dialogue tree that's related to a relationship status
//Used in the dialogue system 
[GlobalClass]
public partial class DialogueFileRelation : DialogueFileR {
    [Export] public RelationshipStatus RelationshipStatus { get; private set; }
}
