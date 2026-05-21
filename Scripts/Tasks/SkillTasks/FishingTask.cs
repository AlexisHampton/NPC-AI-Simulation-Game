using Godot;
using System;

public partial class FishingTask : Task {

    [Export] private FishingPole fishingPole;

    public override void DoTask(NPC npc) {

        //catch a fish after some time
        if (npc.GetTaskStep == 1) {
            Item fish = fishingPole.GetFish();
            npc.AddToBasket(fish, 1);
        }
        base.DoTask(npc);
    }

    public override void FinishTask(NPC npc) {
        npc.SetNextTask(npc.GetPackAwayTask);
        base.FinishTask(npc);
    }
}
