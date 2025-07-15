using Godot;
using System;
using Godot.Collections;

[GlobalClass]
//Tasks checks if an npc can do a task, hold taskR information
public partial class Task : Node3D {

    [Export] protected TaskR taskBase;
    [Export] protected int maxNumNPCs = 1;
    [Export] protected bool isCultureTask = false;
    [Export] protected House house = null;

    protected int numNPCs = 0;
    protected bool isFinished = true;

    public string GetTaskName { get => taskBase.TaskName; }
    public Need GetNeed { get => taskBase.NeedSatisfied; }
    public bool GetIsJobTask { get => taskBase.IsJobTask; }
    public bool GetIsPartnerTask { get => taskBase.IsPartnerTask; }
    public bool GetIsInterruptableTask { get => taskBase.IsInterruptableTask; }
    public bool GetIsChildTask { get => taskBase.IsChildTask; }
    public int GetDuration { get => taskBase.Duration; }
    public bool GetIsFinished { get => isFinished; }
    public bool GetIsCultureTask { get => isCultureTask; }
    public bool GetIsDurationTask { get => taskBase.IsDurationTask; }
    public House GetHouse { get { return house; } set { house = value; } }

    //returns the task's position
    public virtual Vector3 GetTaskPosition() {
        return GlobalPosition;
    }

    //returns a score based only on distance for checking how good this task is
    public float GetTaskScore(Vector3 npcPos, Need incomingNeed, Personality personality = null) {
        int needScore = incomingNeed == taskBase.NeedSatisfied ? 100 : incomingNeed == Need.NONE ? 100 : 0;
        int incompleteScore = !isFinished ? 50 : 0;
        /* int personalityScore = 0;
         if (taskBase.Personality is not null)
             personalityScore = 10 * taskBase.Personality.GetPersonalityMatchScore(personality.personalityDict);
        */
        if (GetIsJobTask) //deter npcs from doing job tasks
            needScore += 100000000;

        return GlobalPosition.DistanceTo(npcPos) - taskBase.Sparkliness - needScore - incompleteScore;
    }

    //Claims the task so other npcs cannot interfere
    public virtual void ClaimTask(NPC npc) {
        numNPCs++;
        isFinished = false;
    }

    //Contains the steps to complete a task
    public virtual void DoTask(NPC npc) {
        GD.PrintS(npc.Name, "is doing", ToString());
    }

    //returns if an npc can do this task
    public virtual bool CheckIfCanDoTask(NPC npc) {
        GD.PrintS(npc.Name, Name, "num NPCs: ", numNPCs);
        return numNPCs < maxNumNPCs;
    }

    //completes the task and makes it available for other npcs
    public virtual void FinishTask(NPC npc) {
        numNPCs--;
        isFinished = true;
    }

    //not implemented
    public virtual Task GetAdjacentTasks(NPC npc) {
        return null;
    }

    public override string ToString() {
        return taskBase.ToString() + " numNPCs: " + numNPCs;
    }


}
