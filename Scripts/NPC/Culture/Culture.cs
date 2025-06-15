using Godot;
using System;
using Godot.Collections;

[GlobalClass]

//Culture is a set of tasks that a group of npcs will tend to do
public partial class Culture : Node3D {

    [Export] public Array<Task> tasks = new();

    public override void _Ready() {
        int numTasks = tasks.Count;

        for (int i = numTasks; i < Globals.Instance.GameTime.GetMaxTimeSlots; i++)
            tasks.Add(tasks[i % numTasks]);
    }
}
