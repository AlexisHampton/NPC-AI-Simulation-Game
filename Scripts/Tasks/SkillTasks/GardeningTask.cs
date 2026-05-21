using Godot;
using System;

public partial class GardeningTask : Task {

    [Export] private GardeningPatch gardeningPatch;
    private bool hasGrown;
    public override bool CheckIfCanDoTask(NPC npc) {
        return base.CheckIfCanDoTask(npc);
    }

    public override void DoTask(NPC npc) {

        if (npc.GetTaskStep == 0) {
            //if seed not planted, plant random seed
            if (!gardeningPatch.HasPlanted) {
                hasGrown = false;
                Item seed = Utilities.GetRandomItem(Globals.Instance.AllSeedsToPlant);
                Item seedToPlant = Utilities.InstantiateItem(seed, Vector3.Zero, this);
                gardeningPatch.PlantSeed((SeedR)seedToPlant.ItemR, seedToPlant);
            }
            //otw harvest the seed
            else if (gardeningPatch.HasPlanted && gardeningPatch.HasSeedGrown) {
                hasGrown = gardeningPatch.HasSeedGrown;
                Item plant = gardeningPatch.HarvestSeedForNPC();
                npc.AddToBasket(plant, 1);
            }
        }

        base.DoTask(npc);
    }

    public override void FinishTask(NPC npc) {
        //pack away crops when done 
        if (hasGrown)
            npc.SetNextTask(npc.GetPackAwayTask);
        base.FinishTask(npc);
    }


}
