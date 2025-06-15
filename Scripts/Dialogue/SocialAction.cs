using Godot;
using System;

[GlobalClass]

//SocialAction holds dialogue options for NPC dialogue choices
public partial class SocialAction : Resource {
    [Export] public string actionName { get; private set; }
    [Export] public RelationshipStatus minRelationshipStatus { get; private set; }
    [Export] public int relationshipAmount { get; private set; }
}
