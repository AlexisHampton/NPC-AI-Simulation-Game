using System;
using Godot;
using Godot.Collections;

public enum PersonalityType { EXTROVERSION, ADVENTUROUSNESS, INTELLECTUALISM }

[GlobalClass]

// Holds an NPCs personality 
public partial class Personality : Resource {
    [Export] public Dictionary<PersonalityType, float> personalityDict = new();

    //Checks if one personality matches another personality
    public int GetPersonalityMatchScore(Dictionary<PersonalityType, float> otherPers) {
        int matches = 0;
        foreach (PersonalityType key in personalityDict.Keys)
            if (otherPers.ContainsKey(key) && otherPers[key] >= personalityDict[key])
                matches++;
        return matches;
    }

    public override string ToString() {
        string str = "";
        foreach (PersonalityType key in personalityDict.Keys)
            str += Enum.GetName(typeof(PersonalityType), key) + " " + personalityDict[key] + ", ";
        return str;
    }

}