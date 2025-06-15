using Godot;

public enum RelationshipStatus { ENEMY = -2, STRAINED, STRANGER, ACQUAINTANCE, FRIEND, BESTFREN, LOVER }
public enum RelationshipType { CHILD, PARENT, FRIEND, STRANGER, SIBLING, LOVER, SPOUSE, ENEMY }

[GlobalClass]
//Holds an NPC's relationship to another NPC
public partial class NPCRelationship : Resource {

    [Export] public string npcName;
    [Export] public RelationshipStatus relationshipStatus = RelationshipStatus.STRANGER;
    [Export] public RelationshipType relationshipType = RelationshipType.FRIEND;
    [Export] public float friendshipAmount;

    //STRANGER: 0-10
    //ACQ = 10-30
    //FREN = 30+
    //BESTFREN = 60+
    //LOVERS  = ROM INTRESET 50+
    //STRAINED -20-0
    //ENEMIES < -20

    public NPCRelationship() { }

    public NPCRelationship(string name) {
        npcName = name;
        friendshipAmount = 0;
        relationshipStatus = RelationshipStatus.STRANGER;
        relationshipType = RelationshipType.STRANGER;
    }

    public void IncreaseFriendshipLevel(float famount) {
        friendshipAmount += famount;

        if (friendshipAmount >= 60)
            relationshipStatus = RelationshipStatus.BESTFREN;
        else if (friendshipAmount >= 30) {
            relationshipStatus = RelationshipStatus.FRIEND;
            relationshipType = RelationshipType.FRIEND;
        } else if (friendshipAmount >= 10)
            relationshipStatus = RelationshipStatus.ACQUAINTANCE;
        else if (friendshipAmount >= 0)
            relationshipStatus = RelationshipStatus.STRANGER;
        else if (friendshipAmount < -20) {
            relationshipStatus = RelationshipStatus.ENEMY;
            relationshipType = RelationshipType.ENEMY;
        } else if (friendshipAmount < 0)
            relationshipStatus = RelationshipStatus.STRAINED;

    }

    public override string ToString() {
        return npcName + " status: " + relationshipStatus + " type: " + relationshipType + " level: " + friendshipAmount;
    }
}
