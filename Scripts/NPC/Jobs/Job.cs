using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;

[GlobalClass]

//Job returns Tasks either in order or randomly
public partial class Job : Node3D {
    [Export] private Array<Task> tasks = new Array<Task>();
    [Export] private bool isSequential;
    //JobR soon
    [Export] private int startTime;
    [Export] private int endTime;
    [Export] private int minPay;
    [Export] private int maxPay;
    [Export] protected Array<DayOfTheWeek> workDays = new Array<DayOfTheWeek>();

    public int GetStartTime { get { return startTime; } }
    public int GetEndTime { get { return endTime; } }
    //public int GetMinPay { get { return minPay; } }
    //public int GetMaxPay { get { return maxPay; } }

    private List<Task> tasksLookedAt = new();

    public int GetPay() {
        return GD.RandRange(minPay, maxPay);
    }

    public bool IsJobDay() {
        return workDays.Contains(Globals.Instance.GameTime.GetDayOfTheWeek);
    }

    //get a job task for an npc
    public Task GetJobTask(NPC npc) {
        if (isSequential)
            return GetSeqJobTask(npc);
        return GetRandJobTask(npc);
    }

    //Get job tasks in order
    private Task GetSeqJobTask(NPC npc) {
        if (npc.GetJobTaskIndex >= tasks.Count || !tasks[npc.GetJobTaskIndex].CheckIfCanDoTask(npc))
            return null;
        return tasks[npc.GetJobTaskIndex];
    }

    //get a random job task
    private Task GetRandJobTask(NPC npc) {
        int randNum = GD.RandRange(0, tasks.Count - 1);
        if (tasksLookedAt.Count >= tasks.Count)
            return null;
        GD.PrintS(npc.Name, "numTasks", tasksLookedAt.Count, "task", tasks[randNum]);
        if (!tasks[randNum].CheckIfCanDoTask(npc)) {
            if (!tasksLookedAt.Contains(tasks[randNum]))
                tasksLookedAt.Add(tasks[randNum]);
            return GetRandJobTask(npc);
        }
        tasksLookedAt.Clear();
        return tasks[randNum];
    }
}
