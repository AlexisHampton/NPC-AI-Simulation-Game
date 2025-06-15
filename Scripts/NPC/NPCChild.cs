using System.Collections.Generic;
using Godot;
using Godot.Collections;

using static Godot.GD;

//An npc that follows parents around and can only do limited tasks
public partial class NPCChild : NPC {

    [Export] private Array<NPC> parents = new();
    [Export] private GetHelpTask getHelpTask;
    [Export] private HelpTask helpTask;

    public override void _Ready() {
        base._Ready();
    }

    //Finds the next task the npc can do
    protected override Task FindNextTask(int time) {
        PrintNeeds();

        List<Task> closest = FindTasksInAreaPlace();
        closest = GetChildTasks(closest);
        PrintS(Name, closest.Count);
        Need lowest = Need.NONE;
        if (!NeedsHighEnough()) {
            lowest = GetLowestNeed();
            npcNeeds[(int)lowest].Amount += 30;
            if (lowest != Need.FUN) {
                NPC parent = FindParent();
                parent.Interupt(helpTask);
                getHelpTask.SetParentAndNeed(lowest, parent);
                return getHelpTask;
            }
        }

        //goals task

        Task bestTask = GetBestTask(closest, lowest);
        if (bestTask != null) {
            //social task
            if (bestTask.GetIsPartnerTask)
                FindSocialPartner(bestTask);
            return bestTask;
        }
        //hobby?

        //favored tasks
        Task favored = DoFavoredTask();
        if (favored != null) {
            PrintS(Name, "is doing favored task");
            return favored;
        }

        //do non mandatory cultureTask
        Task cultureTask = culture.tasks[time];
        if (cultureTask.CheckIfCanDoTask(this)) {
            PrintS(Name, "is doing a culture task that is not mandatory");
            return cultureTask;
        }

        return null;
    }

    //Finds culture tasks or school tasks
    protected override Task FindMandatoryTask(int time) {
        //culture task
        Task cultureTask = culture.tasks[time];
        if (cultureTask.GetIsCultureTask && cultureTask.CheckIfCanDoTask(this)) {
            PrintS(Name, "is doing mandatory Culture Task");
            return cultureTask;
        }

        //school task
        //if time is during school time, find school task
        if (school != null && school.IsJobDay() && time >= school.GetStartTime && time < school.GetEndTime) {
            jobTaskIndex++;
            Task schoolTask = school.GetJobTask(this);
            if (schoolTask != null) {
                PrintS(Name, "is doing school task");
                if (schoolTask is HelpChildTask helpChildTask) {
                    PrintS(Name, "is doing getting help doing a school task");
                    NPC parent = FindParent();
                    parent.Interupt(helpChildTask);
                    helpChildTask.SetChild(this);
                    FollowCharacter(parent);
                }
                return schoolTask;
            }
        }

        jobTaskIndex = -1;
        return null;
    }

    //gets all tasks that the child is capable of doing
    private List<Task> GetChildTasks(List<Task> closest) {
        List<Task> tasks = new List<Task>();

        foreach (Task task in closest)
            if (task.GetIsChildTask)
                tasks.Add(task);
        return tasks;
    }

    //finds the closest parent distance-wise
    private NPC FindParent() {
        if (parents.Count > 2)
            return parents[0];
        NPC closestParent = parents[0];
        float smallestDistance = float.MaxValue;
        foreach (NPC parent in parents) {
            if (parent.GetCurrTask.GetIsJobTask) continue;
            float distance = GlobalPosition.DistanceTo(parent.GlobalPosition);
            if (smallestDistance > distance) {
                closestParent = parent;
                smallestDistance = distance;
            }
        }

        return closestParent;
    }

}
