using Godot;
using System;

//PlayerTalkTask allows an npc to talk to the player 
public partial class PlayerTalkTask : Task {

    public override bool CheckIfCanDoTask(NPC npc) {
        if (Globals.Instance.Player.IsBusy) return false;
        return base.CheckIfCanDoTask(npc);
    }
    public override void DoTask(NPC npc) {
        if (npc.GetTaskStep == 0)
            npc.TalkToPlayer();

        base.DoTask(npc);
    }

    public override void FinishTask(NPC npc) {
        base.FinishTask(npc);
    }
}
