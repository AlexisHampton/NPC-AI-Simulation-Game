using Godot;
using Godot.Collections;

//SitAndOrderTask is a TavernTask where npcs order food and wait for it
public partial class SitAndOrderTask : TavernTask {

    [Export] private Sittable chair;
    [Export] private Node3D foodPos;
    [Export] private EatSittingTask eatTask;

    [Export] private Array<Sittable> chairs;
    [Export] private Array<Node3D> foodPositions;
    //[Export] private Array<EatSittingTask> eatSittingTasks;

    private Item item = null;
    private bool hasOrderTaken = false;
    private bool npcsAtTable = false;
    //private int chairsSatIn = 0;
    private Dictionary<NPC, int> npcsInChairs = new Dictionary<NPC, int>();


    //time to wait before they leave hungry
    private int maxTimeSinceServed = 7;

    //claim the table
    public override void ClaimTask(NPC npc) {
        base.ClaimTask(npc);
        npcsAtTable = true;
    }

    public void SetOrderTaken(bool orderTaken) {
        hasOrderTaken = orderTaken;
    }

    //put food on table
    public void SetFood(Item food) {
        item = food;
        food.Reparent(foodPos);
        food.Position = Vector3.One;
        GD.PrintS("food, parent", food.GetParent().Name);
    }

    //if tavern is not open or people are already sitting there, leave
    public override bool CheckIfCanDoTask(NPC npc) {
        if (!tavern.GetIsOpen) return false;
        if (npcsAtTable) return false;
        return base.CheckIfCanDoTask(npc);
    }

    public override void DoTask(NPC npc) {
        //if the tavern closes while still inside, leave
        if (!tavern.GetIsOpen || npc.GetTaskStep > maxTimeSinceServed) {
            GD.PrintS(npc.Name, "leaving tavern because sittingAndEating", npc.GetTaskStep, "time since served, chairs sat in");
            npcsAtTable = false;
            if (npcsInChairs.ContainsKey(npc))
                chairs[npcsInChairs[npc]].StandUp(npc);
            npcsInChairs.Clear();
            base.FinishTask(npc);
            return;
        }
        //sit npcs down
        if (npc.GetTaskStep == 0) {
            GD.PrintS(npc.Name, Name, "dict has:", npcsInChairs.ToString(), Time.GetTicksMsec());
            for (int i = 0; i < chairs.Count; i++) {
                if (npcsInChairs.Values.Contains(i)) continue;
                if (npcsInChairs.ContainsKey(npc)) continue;
                chairs[i].SitDown(npc);
                npcsInChairs.Add(npc, i);
                tavern.AddCustomerToLine(npc);
            }
        }

        if (item != null)
            FinishTask(npc);

        base.DoTask(npc);
    }

    //make npcs eat their food
    public override void FinishTask(NPC npc) {
        //hasOrderTaken = false;
        if (!npcsAtTable) return;
        npcsAtTable = false;
        eatTask.SetItemToEat(item);
        eatTask.SetChair(chair);
        npc.SetNextTask(eatTask);
        npcsInChairs.Clear();
        base.FinishTask(npc);
    }
}
