using Godot;
using System;

public enum AgeStage { BABY, CHILD, ADULT }

//NPCAge holds the life stage and progression toward the next for an NPC
public partial class NPCAge : Resource {
    [Export] public AgeStage ageStage;
    [Export] public float ageProgression;
}
