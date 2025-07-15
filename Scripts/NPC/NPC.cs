using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

using static Godot.GD;

//NPCs move to desired locations, preform tasks, and pick things up
public partial class NPC : CharacterBody3D, ITaskHolder {

    [ExportGroup("NPC")]
    [Export] protected Label3D nameLabel;
    [Export] protected Label3D taskLabel;
    [Export] protected NPCAge age;
    [Export] protected Godot.Collections.Dictionary<NPC, NPCRelationship> npcRelationships = new Godot.Collections.Dictionary<NPC, NPCRelationship>();
    [Export] protected House house;

    [ExportGroup("Inventory")]
    [Export] protected Inventory basket;
    [Export] protected Job job;
    [Export] protected Job school;


    [ExportGroup("Tasks")]
    [Export] protected Array<Task> schedule = new Array<Task>();
    [Export] protected Array<Task> favoredTasks = new Array<Task>();
    [Export] protected Culture culture;
    [Export] protected TalkTask talkTask;

    [ExportGroup("Movement")]
    [Export] protected Array<RayCast3D> raycasts = new Array<RayCast3D>();
    [Export] protected ShapeCast3D shapecast;
    [Export] protected NavigationAgent3D agent;
    [Export] protected float speed = 10;
    [Export] protected float rotationSpeed = 10;
    [Export] protected Timer followTimer;
    //tasks
    protected NPCNeed[] npcNeeds = new NPCNeed[4];
    protected Task currTask;
    protected Task nextTask; //to be set by external tasks
    protected Task interruptedTask;
    protected bool isDoingTask = false;
    protected int timeLeftOnTask = 0;
    protected int taskStep = 0;

    //job task
    public int JobTaskIndex { get; private set; }

    //determines whether npc is following another character
    protected bool isFollowing;
    protected NPC npcToFollow;

    public Task GetCurrTask { get => currTask; }
    public int GetTaskStep { get => taskStep; }

    private Vector3 pastVel = new Vector3(-10000, -10000, -10000);

    //initialize needs, schedule and house
    public override void _Ready() {
        nameLabel.Text = Name;
        for (int i = 0; i < npcNeeds.Length; i++) {
            Need need = (Need)Enum.Parse(typeof(Need), i.ToString());
            int randAmt = RandRange(90, 100);
            npcNeeds[i] = new NPCNeed(need, randAmt);
        }

        //init scehdule
        for (int i = 0; i < Globals.Instance.GameTime.GetMaxTimeSlots; i++)
            schedule.Add(null);

        //set house
        house.SetNPCOwner(this);

        JobTaskIndex = -1;
        GameEvents.OnTimeIncrease += DecreaseAllNeeds;
        GameEvents.OnTimeIncrease += NextTask;

        // NextTask(0);
    }

    #region AI Navigation Movement

    //NPC moves to destination every physics frame
    public override void _PhysicsProcess(double delta) {
        //do interaction


        //if following other character
        if (isFollowing) {
            if (GlobalPosition.DistanceTo(currTask.GlobalPosition) > 1)
                isDoingTask = false;
        }

        //if at destination
        if (agent.IsNavigationFinished()) {
            if (isDoingTask && currTask != null) {
                currTask.DoTask(this);
                taskStep++;
                isDoingTask = false;
            }
            return;
        }

        //get the next location to move to
        Vector3 nextLoc = agent.GetNextPathPosition();
        Vector3 offset = nextLoc - GlobalPosition;
        Vector3 newVel = offset.Normalized() * speed;

        //If the npc is stuck, teleport to the next position
        if (Velocity == pastVel) {
            GlobalPosition = nextLoc;
            return;
        }

        agent.Velocity = newVel;
        pastVel = Velocity;

        MoveAndSlide();

        //rotate to face where its going
        offset.Y = 0;
        if (GlobalTransform.Origin.IsEqualApprox(GlobalPosition + offset))
            return;
        LookAt(GlobalPosition + offset, Vector3.Up);
    }


    //Move npc to specified position
    public void Move(Vector3 pos) {
        agent.TargetPosition = pos;
    }

    //don't collide with other things and hopefully npcs
    public void OnNavigationAgent3DVelocityComputed(Vector3 safeVel) {
        Velocity = safeVel;
        MoveAndSlide();
    }

    //NPC updates its pathfinding to follow specific npc 
    public void FollowCharacter(NPC npc) {
        npcToFollow = npc;
        if (isFollowing) return;
        followTimer.Timeout += UpdatePathfinding;
        followTimer.Start();
        isFollowing = true;
    }

    //Sets target position for npc to trail behind at a good distance
    public void UpdatePathfinding() {
        agent.TargetPosition = npcToFollow.GlobalPosition + new Vector3(1, 0, 1);
    }

    //Resets NPC to stop following an npc
    public void StopFollow() {
        if (!isFollowing) return;

        npcToFollow = null;
        followTimer.Timeout -= UpdatePathfinding;
        isFollowing = false;
    }

    #endregion
    //--------------NPC AI-------------------

    //returns if npc is preoccupied
    public bool IsDoingTask() => isDoingTask;

    #region NextTask

    //Gets the next task for the npc to do
    protected void NextTask(int time) {
        PrintS(Name, timeLeftOnTask, currTask?.Name);

        if (currTask != null) {
            isDoingTask = true;
            //check if already doing task and if duration task
            if (currTask.GetIsDurationTask && timeLeftOnTask > 0) {
                timeLeftOnTask--;
                return;
            }
            //if it's not a duration task and isInProgress
            if (!currTask.GetIsDurationTask && !currTask.GetIsFinished)
                return;
        }
        currTask?.FinishTask(this);

        //get cached next task
        currTask = schedule[time];
        PrintS(Name, "is looking for a task");

        //if npc was interrupted
        if (interruptedTask != null) {
            currTask = interruptedTask;
            PrintS(Name, "interrupted task is:", currTask.Name);
            interruptedTask = null;
        }

        //if there is a mandatory task available: 
        Task mandatoryTask = FindMandatoryTask(time);
        if (mandatoryTask != null) {
            currTask = mandatoryTask;
        }

        //if a task has set a next task
        if (nextTask != null) {
            currTask = nextTask;
            PrintS(Name, "next task is:", currTask.Name);
            nextTask = null;
        }

        //if dont have a cached task, find a task and cache it
        if (currTask == null) {
            currTask = FindNextTask(time);
            schedule[time] = currTask;
            PrintS(Name, currTask.ToString());

        }
        //go to the task
        ClaimTask(currTask);
    }

    //NPC claims the task they will do next so no other npc can do it
    protected void ClaimTask(Task task) {
        currTask = task;
        currTask.ClaimTask(this);
        timeLeftOnTask = currTask.GetDuration;
        taskStep = 0;
        taskLabel.Text = currTask.GetTaskName;

        if (!isFollowing)
            Move(currTask.GetTaskPosition());

        isDoingTask = true;
    }

    //finds the next task 
    protected virtual Task FindNextTask(int time) {
        //PrintNeeds();
        //need task

        List<Task> closest = FindTasksInAreaPlace();

        Need lowest = Need.NONE;
        if (!NeedsHighEnough()) {
            lowest = GetLowestNeed();
            npcNeeds[(int)lowest].Amount += 30;
        }

        //goals task
        Task bestTask = GetBestTask(closest, lowest);
        if (bestTask != null)
            PrintS("bestTask is ", bestTask.ToString());

        if (bestTask != null && bestTask.GetIsPartnerTask) {
            PrintS("bestTask is ", bestTask.ToString());
            //social task
            NPC partner = FindSocialPartner(bestTask);
            if (bestTask is TalkTask && partner is not null)
                return bestTask;
            else if (bestTask is not TalkTask && partner is null)
                return bestTask;
        }
        //hobby?

        //favored tasks
        /*
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
*/

        PrintS(Name, "is looking for a home task", house.GetPlaceTasks().Count, house.Name);
        Task homeTask = GetBestTask(house.GetPlaceTasks(), lowest);
        if (homeTask is not null && homeTask.CheckIfCanDoTask(this)) {
            PrintS(Name, "is doing a home task as a last resort");
            return homeTask;
        }

        return null;
    }

    //Finds a task that the NPC has to do like going to work, school, or a culture task like going to church
    protected virtual Task FindMandatoryTask(int time) {
        //culture task is not implemented yet in the new version
        /*        Task cultureTask = culture.tasks[time];
                if (cultureTask.GetIsCultureTask && cultureTask.CheckIfCanDoTask(this)) {
                    PrintS(Name, "is doing mandatory Culture Task");
                    return cultureTask;
                }
        */
        //job task
        //if time is during job time, find job task
        if (job != null && job.IsJobDay() && time >= job.GetStartTime && time < job.GetEndTime) {
            JobTaskIndex++;
            Task jobTask = job.GetJobTask(this);
            if (jobTask != null) {
                PrintS(Name, "is doing job task");
                return jobTask;
            }
        }

        //school task
        //if time is during school time, find school task
        if (school != null && school.IsJobDay() && time >= school.GetStartTime && time < school.GetEndTime) {
            JobTaskIndex++;
            Task schoolTask = school.GetJobTask(this);
            if (schoolTask != null) {
                PrintS(Name, "is doing school task");
                return schoolTask;
            }
        }

        JobTaskIndex = -1;
        return null;
    }

    //For tasks to set next tasks.
    //i.e. getting a checkout task after enough browsing shelves tasks
    public void SetNextTask(Task nextTaskIn) {
        nextTask = nextTaskIn;
    }

    //pick a random favorite task
    protected Task DoFavoredTask() {
        int randNum = RandRange(0, favoredTasks.Count - 1);
        Task task = favoredTasks[randNum];
        if (task.CheckIfCanDoTask(this))
            return task;
        return null;
    }

    #endregion

    #region GetBestTask

    //Gets the best need task based on surrounding tasks
    protected Task GetBestTask(List<Task> closest, Need lowest) {
        PriorityQueue<Task, float> potentialTasks = new();

        foreach (Task task in closest)
            potentialTasks.Enqueue(task, task.GetTaskScore(GlobalPosition, lowest));

        potentialTasks.TryDequeue(out Task bestTask, out _);

        //no tasks
        if (bestTask == null) return null;

        while (!bestTask.CheckIfCanDoTask(this)) {
            potentialTasks.TryDequeue(out bestTask, out _);

            //if no more tasks
            if (bestTask == null)
                break;
        }

        return bestTask;
    }

    //interrupts npc from current task and gives them a new task
    public void Interupt(Task newTask) {
        interruptedTask = currTask;
        //if (currTask is SitTask) currTask.FinishTask(this);
        SetNextTask(newTask);
    }

    #endregion

    #region Needs
    //Finds the lowest need
    protected Need GetLowestNeed() {
        Need lowest = Need.NONE;
        int amt = int.MaxValue;

        foreach (NPCNeed n in npcNeeds)
            if (amt > n.Amount) {
                lowest = n.need;
                amt = n.Amount;
            }

        return lowest;

    }
    //checks if the needs are high enough to skip
    protected bool NeedsHighEnough() {
        foreach (NPCNeed npcNeed in npcNeeds)
            if (npcNeed.Amount < 75)
                return false;
        return true;
    }

    //decreases all needs every time increase
    protected void DecreaseAllNeeds(int time) {
        foreach (NPCNeed npcNeed in npcNeeds)
            npcNeed.Amount -= 20;//1;

    }

    //prints Needs
    protected void PrintNeeds() {
        string needs = Name + " ";
        foreach (NPCNeed n in npcNeeds) {
            needs += n.need.ToString() + " " + n.Amount + " ";
        }
        Print(needs);
    }

    #endregion

    #region FindTasks

    //Finds all the tasks around the npc
    protected List<Task> FindTasksInAreaPlace() {
        List<Task> closest = new List<Task>();
        if (!shapecast.IsColliding())
            return closest;
        for (int i = 0; i < shapecast.GetCollisionCount(); i++) {
            //finds tasks in commercial spaces
            if (shapecast.GetCollider(i) is Area3D area && area.GetParent() is CommercialSpace commSpace) {
                foreach (Task task in commSpace.GetPlaceTasks()) {
                    if (!task.GetIsJobTask) {
                        closest.Add(task);
                    }
                }
                //finds tasks in ITaskHolders i.e. NPCs
            } else if (shapecast.GetCollider(i) is ITaskHolder taskHolder) {
                foreach (Task task in taskHolder.GetTasks()) {
                    if (!task.GetIsJobTask) {
                        closest.Add(task);
                    }
                }
            }
        }

        return closest;
    }

    //Finds a friend for the npc to do a partner task with
    protected NPC FindSocialPartner(Task partnerTask) {
        NPC npcToTalkTo = null;

        //random chance will be replaced with personality in future iterations
        float randNum = Randf();
        //Find NPC
        //30% chance of an npc looking through their current relationships to find a friend
        if (randNum > 0.3 && npcRelationships?.Count != 0)
            foreach (NPC npc in npcRelationships.Keys) {
                if (npc.currTask is not null && !npc.currTask.GetIsInterruptableTask) continue;
                npcToTalkTo = npc;
                break;
            }
        else { //70% chance of an npc to find a random npc in the vicinity
            if (!shapecast.IsColliding()) return null;
            for (int i = 0; i < shapecast.GetCollisionCount(); i++) {
                if (shapecast.GetCollider(i) is not NPC npc) continue;
                if (npc.currTask is not null && !npc.currTask.GetIsInterruptableTask) continue;
                npcToTalkTo = npc;
                break;
            }
        }

        if (npcToTalkTo == null) return null;

        PrintS(Name, "and", npcToTalkTo.Name, "is doing partner task", partnerTask.GetTaskName);

        ClaimTask(partnerTask);
        npcToTalkTo.ClaimTask(partnerTask);

        return npcToTalkTo;
    }


    #endregion

    #region Relationships

    //Updates NPC relationships with other npcs
    public void UpdateNPCRelationship(NPC npc, float relationshipLevel) {
        if (npcRelationships.ContainsKey(npc)) {
            npcRelationships[npc].IncreaseFriendshipLevel(relationshipLevel);
            PrintS(Name, "social", npcRelationships[npc].ToString());
            return;
        } else {//if no relationship
            NPCRelationship newRelationship = new NPCRelationship(npc.Name);
            npcRelationships.Add(npc, newRelationship);
            PrintS(Name, "new social rel ", newRelationship.ToString());
        }

    }


    //Returns an npc relationship for the specified npc
    public NPCRelationship GetNPCRelationship(NPC npc) {
        return npcRelationships.ContainsKey(npc) ? npcRelationships[npc] : null;
    }

    #endregion
    //-----------------------Inventory-------------------------------
    #region Inventory
    //---Basket---
    // the basket concept might be deprecated in future versions
    /*
    //Checks if the basket is empty
    public bool IsBasketEmpty() {
        return basket.IsEmpty();
    }

    public bool IsBasketFull() {
        return basket.IsFull();
    }

    //Adds an amount of itemR to the basket
    public void AddToBasket(Item item, int amt) {
        basket.Visible = true;
        basket.AddToInventory(item, amt);
    }

    //Removes an amount of the specified itemR from the basket
    public void RemoveFromBasket(ItemR item, int amt) {
        basket.RemoveFromInventory(item, amt);
        if (IsBasketEmpty())
            basket.Visible = false;
    }
    */

    //Return all tasks on the NPC
    public Array<Task> GetTasks() {
        Task[] tasks = [talkTask];
        return new Array<Task>(tasks);
    }


    //updates the task label for the task the npc is doing
    public void UpdateTaskLabel(string taskMessage) {
        taskLabel.Text += " " + taskMessage;
    }

    //returns a string of the current, next, and interrupted task names
    public string PrintNextInterruptableTask() {
        return " ct: " + currTask?.Name + " nt: " + nextTask?.Name + " it: " + interruptedTask?.Name;
    }
    #endregion
}
