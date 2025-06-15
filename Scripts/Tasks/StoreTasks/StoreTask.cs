using Godot;
using System;

//StoreTask gets information from the store to preform it's tasks
public partial class StoreTask : Task {
    [Export] protected Store store;

    public Store GetStore { get => store; }
    public void SetStore(Store storeIn) {
        store = storeIn;
    }

    //Checks if the npc is allowed to do a task if working
    public override bool CheckIfCanDoTask(NPC npc) {
        if (!store.IsEmployee(npc) && !store.GetIsOpen && !GetIsJobTask) return false;
        return base.CheckIfCanDoTask(npc);
    }

    public override void FinishTask(NPC npc) {
        base.FinishTask(npc);
    }
}
