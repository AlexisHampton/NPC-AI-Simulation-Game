using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;


//Place holds task information, owner information
public partial class Place : Node3D {

    [Export] protected Label3D nameLabel;
    [Export] protected Array<Task> placeTasks = new Array<Task>();

    protected List<NPC> npcOwners = new List<NPC>();

    public override void _Ready() {
        nameLabel.Text = "";
    }

    //Returns all the place tasks as a list instead of a Godot.Collections.Array<>
    public List<Task> GetPlaceTasks() {
        List<Task> placeTs = new List<Task>();
        foreach (Task t in placeTasks) {
            placeTs.Add(t);
        }
        return placeTs;
    }

    //Sets the owner of the place to the specified NPC
    public void SetNPCOwner(NPC npc) {

        if (!IsNPCOwner(npc)) {
            if (this is House)
                nameLabel.Text += npc.Name + " ";
            npcOwners.Add(npc);
        }
    }

    //Check if specified NPC is an owner
    public bool IsNPCOwner(NPC npc) {
        return npcOwners.Contains(npc);
    }

    //Adds a specified task to placeTasks
    public virtual void AddTask(Task task) {
        placeTasks.Add(task);
    }


    public override bool Equals(object obj) {
        if (obj is not House) return false;
        House other = (House)obj;
        foreach (NPC npc in npcOwners)
            if (!other.IsNPCOwner(npc))
                return false;
        return true;
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }


}
