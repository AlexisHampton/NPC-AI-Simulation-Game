using Godot;
using Godot.Collections;

public static partial class RelationshipManager {

    public const int FRIENDSHIP_LEVEL_UPDATE_BASE = 10;

    //Updates the NPCRelationship by the given relationshipLevel
    public static void UpdateNPCRelationship(string name, Dictionary<NPC, NPCRelationship> npcRelationships, NPC npc, float relationshipLevel = FRIENDSHIP_LEVEL_UPDATE_BASE) {
        if (npcRelationships.ContainsKey(npc)) {
            npcRelationships[npc].IncreaseFriendshipLevel(relationshipLevel);
            //GD.PrintS(name, "social", npcRelationships[npc].ToString());
            return;
        } else {//if no relationship
            NPCRelationship newRelationship = new NPCRelationship(npc.Name);
            npcRelationships.Add(npc, newRelationship);
            //GD.PrintS(name, "new social rel ", newRelationship.ToString());
        }

    }


    //returns an NPCRelationship for the specified npc
    public static NPCRelationship GetNPCRelationship(Dictionary<NPC, NPCRelationship> npcRelationships, NPC npc) {
        return npcRelationships.TryGetValue(npc, out NPCRelationship rel) ? rel : null;
    }

    //Updates the player's relationship level by amount, pos increases, neg decreases
    public static void IncreasePlayerRelationship(string name, NPCRelationship playerRelationship, float amount, Label3D relationshipLevelLabel) {
        playerRelationship.IncreaseFriendshipLevel(amount);
        string relationshipLevel = amount > 0 ? "++" : "--";
        relationshipLevelLabel.Text = relationshipLevel;
        relationshipLevelLabel.Visible = true;

        //GD.PrintS(name, "player relationship", playerRelationship.ToString());

    }

}
