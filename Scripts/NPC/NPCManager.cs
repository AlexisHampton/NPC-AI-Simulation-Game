using Godot;
using System.Collections.Generic;
using System.Data.Common;

//NPCManager provides optimization, and stuff that npcs need in one place
public partial class NPCManager : Node3D {

    [ExportGroup("Debug")]
    [Export] private int chatRelationshipIncreaser = 10;
    [Export] private int askRelationshipIncreaser = 5;
    [Export] private int giftRelationshipIncreaser = 40;
    [Export] private int insultRelationshipIncreaser = -10;
    [Export] private int npcChatRelationshipIncreaser = 20;
    [Export] private float minDistToPlayerBase = 30;


    private List<NPC> npcs = new List<NPC>();

    private static float MinDistToPlayer = 30;
    //increases realtionship amount when speaking
    public static int ChatRelationshipIncreaser { get; private set; }
    public static int AskRelationshipIncreaser { get; private set; }
    public static int GiftRelationshipIncreaser { get; private set; }
    public static int InsultRelationshipIncreaser { get; private set; }
    public static int NPCChatRelationshipIncreaser { get; private set; }

    public override void _Ready() {
        foreach (Node node in GetChildren()) {
            if (node is NPC npc)
                npcs.Add(npc);
        }

        //set up static vars
        ChatRelationshipIncreaser = chatRelationshipIncreaser;
        AskRelationshipIncreaser = askRelationshipIncreaser;
        InsultRelationshipIncreaser = insultRelationshipIncreaser;
        GiftRelationshipIncreaser = giftRelationshipIncreaser;
        NPCChatRelationshipIncreaser = npcChatRelationshipIncreaser;

        MinDistToPlayer = minDistToPlayerBase;
    }

    public override void _PhysicsProcess(double delta) {
        foreach (NPC npc in npcs)
            npc.Update(delta);
    }

    //Returns the playerTalkTask if the npc is within talking distance  to the player
    public static PlayerTalkTask CheckInPlayerArea(NPC npc, int modifier) {
        if (modifier <= 0) modifier = 1;
        PlayerTalkTask ptt = Globals.Instance.Player.PlayerTalkTask;
        float distanceToPlayer = npc.GlobalPosition.DistanceTo(Globals.Instance.Player.GlobalPosition);
        if (distanceToPlayer > MinDistToPlayer * modifier || !ptt.CheckIfCanDoTask(npc))
            return null;
        ptt.ClaimTask(npc);
        return ptt;
    }


}
