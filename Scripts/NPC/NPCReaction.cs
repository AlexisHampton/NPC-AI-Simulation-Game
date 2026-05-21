using Godot;
using Godot.Collections;

//NPCReaction holds the dialogue and/or task reaction that an NPC can have to the player, other npcs, or an events
[GlobalClass]
public partial class NPCReaction : Resource {

    [Export] public Array<DialogueFileRelation> DialogueRelations { get; private set; } = new();
    [Export] public TaskR Task { get; private set; }
}
