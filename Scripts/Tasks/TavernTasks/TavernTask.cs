using Godot;
using System;

//TavernTask gets information from the tavern to preform it's tasks
public partial class TavernTask : Task {
    [Export] protected Tavern tavern;

    public void SetStore(Tavern tav) {
        tavern = tav;
    }

    //Checks if the npc is allowed to do a task if working
    public override bool CheckIfCanDoTask(NPC npc) {
        if (!tavern.IsEmployee(npc) && !tavern.GetIsOpen && !GetIsJobTask) return false;
        return base.CheckIfCanDoTask(npc);
    }
}
