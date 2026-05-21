using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

//will likely be deprecated since it causes lag
public static partial class TaskManager {

    //Gets rid of the tasks that have a different taskR
    public static List<Task> FindTasksWithSameSignature(List<Task> closest, TaskR taskFilter) {
        //cull the ones with a different TaskR
        closest.RemoveAll(task => !task.HasSameSignature(taskFilter));
        // foreach (Task task in closest)
        //     if (!task.HasSameSignature(taskFilter))
        //         closest.Remove(task);

        return closest;
    }
    //Finds all the tasks around the npc
    public static List<Task> FindTasksInAreaPlace(string name, ShapeCast3D taskShapecast) {
        taskShapecast.Enabled = true;
        List<Task> closest = new List<Task>();

        if (!taskShapecast.IsColliding())
            return closest;
        for (int i = 0; i < taskShapecast.GetCollisionCount(); i++) {
            if (taskShapecast.GetCollider(i) is Area3D area && area.GetParent() is CommercialSpace commSpace) {
                foreach (Task task in commSpace.GetPlaceTasks()) {
                    if (!task.GetIsJobTask) {
                        // GD.PrintS(name, "place task", task.Name, commSpace.Name);
                        closest.Add(task);
                    }
                }
            } else if (taskShapecast.GetCollider(i) is ITaskHolder taskHolder) {
                foreach (Task task in taskHolder.GetTasks()) {
                    if (!task.GetIsJobTask) {
                        //GD.PrintS(name, "task holder task", task.Name);
                        closest.Add(task);
                    }
                }
            }
        }
        taskShapecast.Enabled = false;

        return closest;
    }

    //Gets the best need task based on surrounding tasks
    public static Task GetBestTask(NPC npc, List<Task> closest, Need lowest, Vector3 globalPosition, Personality personality) {
        PriorityQueue<Task, float> potentialTasks = new();

        foreach (Task task in closest) {
            potentialTasks.Enqueue(task, task.GetTaskScore(globalPosition, lowest, personality));
        }

        potentialTasks.TryDequeue(out Task bestTask, out _);

        //no tasks
        if (bestTask is null) return null;

        while (!bestTask.CheckIfCanDoTask(npc)) {
            potentialTasks.TryDequeue(out bestTask, out _);

            //if no more tasks
            if (bestTask is null)
                break;
        }

        return bestTask;
    }


    //if time is during job time, find job task
    public static Task GetJobTask(NPC npc, Job job, int time) {
        if (job is null || !job.IsJobTime(time)) return null;
        return job.GetJobTask(npc);
    }


    //pick a random favorite task
    public static Task DoFavoredTask(Array<Task> favoredTasks, NPC npc) {
        int randNum = GD.RandRange(0, favoredTasks.Count - 1);
        Task task = favoredTasks[randNum];
        if (task.CheckIfCanDoTask(npc))
            return task;
        return null;
    }
}
