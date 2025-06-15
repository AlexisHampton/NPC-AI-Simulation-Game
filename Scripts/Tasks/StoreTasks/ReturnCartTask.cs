using Godot;
using System;

//Returns cart to original position
public partial class ReturnCartTask : StoreTask {

    public override bool CheckIfCanDoTask(NPC npc) {
        return base.CheckIfCanDoTask(npc);
    }

    public override void DoTask(NPC npc) {
        if (npc.GetTaskStep == 0) {
            //stand up
            npc.GetCart.TurnOffCollision(true);
            npc.GetCart.GetOutOfSeat(npc);
            npc.GiveRemoteControl(false);
            //NavigationPaths.Instance.BakeNavMesh();
            FinishTask(npc);
        }


        base.DoTask(npc);
    }

    public override void FinishTask(NPC npc) {
        base.FinishTask(npc);
    }
}
