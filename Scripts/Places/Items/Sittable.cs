using Godot;
using Godot.Collections;

[GlobalClass]

//Sittable contains a place for an npc to sit and stand up from
public partial class Sittable : Node3D, ITaskHolder {

    [Export] private bool isUsed = false;
    [Export] private Node3D topOfChairPos;
    [Export] private Node3D sideOfChairPos;
    [Export] private Array<Task> tasks;

    private Node prevParent;
    public bool GetIsUsed { get => isUsed; }

    //an NPC can claim a seat so no one else can sit on it
    public void ClaimSeat(bool isOn = true) {
        isUsed = isOn;
    }

    public Array<Task> GetTasks() {
        return tasks;
    }

    //A npc is reparented and positioned at the topOfChair position
    public void SitDown(NPC npc) {
        ClaimSeat();
        prevParent = npc.GetParent();
        prevParent.RemoveChild(npc);
        AddChild(npc);
        npc.GlobalPosition = topOfChairPos.GlobalPosition;
    }

    //An npc's position is set to sideOfChair's position and reparented to the last parent
    public void StandUp(NPC npc) {
        ClaimSeat(false);
        if (prevParent is null) return; //if no parent, npc is likely not sitting at all

        npc.Reparent(prevParent);
        npc.GlobalPosition = sideOfChairPos.GlobalPosition;
    }

}
