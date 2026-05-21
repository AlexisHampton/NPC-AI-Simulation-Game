using Godot;
using System;

public partial class MaterialGatheringTask : Task {

    [Export] MaterialGatherers materialGatherer;
    public override bool CheckIfCanDoTask(NPC npc) {
        if (materialGatherer.IsDepleted) return false;
        return base.CheckIfCanDoTask(npc);
    }

    public override void DoTask(NPC npc) {
        //if finish gathering, get the item
        if (materialGatherer.IsDepleted) {
            Item item = materialGatherer.GetItem();
            npc.AddToBasket(item, 1);
            base.FinishTask(npc);
            return;
        }

        //Hit 4x to make it go faster
        materialGatherer.Hit(4);
        base.DoTask(npc);
    }

    public override void FinishTask(NPC npc) {
        npc.SetNextTask(npc.GetPackAwayTask);
        base.FinishTask(npc);
    }
}
