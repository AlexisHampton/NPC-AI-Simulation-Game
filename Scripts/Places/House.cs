using Godot;
using System;
using Godot.Collections;

[GlobalClass]

//House is a Place where an NPC lives
public partial class House : Place {

    [Export] private PackAwayTask packAwayTask;

    public PackAwayTask GetPackAwayTask { get => packAwayTask; }

    public override void _Ready() {
        //npcOwnerName = npcOwner.Name;
        base._Ready();
        foreach (Task task in placeTasks)
            task.GetHouse = this;
    }

    //when furniture is added to a house, add the task
    public override void AddTask(Task task) {
        base.AddTask(task);
        task.GetHouse = this;
    }

}
