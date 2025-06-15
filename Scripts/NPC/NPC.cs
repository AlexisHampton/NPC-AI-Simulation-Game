using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using static Godot.GD;

//NPCs move to desired locations, preform tasks, and pick things up
public partial class NPC : CharacterBody3D, ITaskHolder {

    [ExportGroup("NPC")]
    [Export] protected Label3D nameLabel;
    [Export] protected Label3D taskLabel;
    [Export] protected NPCAge age;
    [Export] protected Personality personality;
    [Export] protected Godot.Collections.Dictionary<NPC, NPCRelationship> npcRelationships = new Godot.Collections.Dictionary<NPC, NPCRelationship>();
    [Export] protected int money;
    [Export] protected House house;
    [Export] protected Job job;
    [Export] protected Job school;

    [ExportGroup("Inventory")]
    //for one item
    [Export] protected Hands hands;
    //for many items
    [Export] protected DynamicInventory basket;
    //for crates; optional
    [Export] protected Driveable driveable;

    [ExportGroup("Tasks")]
    [Export] protected Array<Task> schedule = new Array<Task>();
    [Export] protected Task hobby;
    [Export] protected Array<Task> favoredTasks = new Array<Task>();
    [Export] protected Culture culture;
    [Export] protected TalkTask talkTask;

    [ExportGroup("Movement")]
    [Export] protected Array<RayCast3D> raycasts = new Array<RayCast3D>();
    [Export] protected ShapeCast3D shapecast;
    [Export] protected NavigationAgent3D agent;
    [Export] protected float speed = 10;
    [Export] protected float rotationSpeed = 10;
    [Export] protected CollisionShape3D collision;
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
    protected int jobTaskIndex;
    //determines whether npc can move on their own, or if something else is moving it, i.e. driving, or taking the bus
    protected bool isRemoteControlled = false;
    //determines whether npc is following another character
    protected bool isFollowing;
    protected NPC npcToFollow;

    public Task GetCurrTask { get => currTask; }
    public int GetTaskStep { get => taskStep; }
    public int SetTaskStep { set => taskStep = value; }
    public int GetJobTaskIndex { get => jobTaskIndex; }
    public Job GetJob { get => job; }
    public Driveable GetCart { get => driveable; }
    public PackAwayTask GetPackAwayTask { get => house.GetPackAwayTask; }

    private Rid navMapRID;
    private Vector3 pastVel = new Vector3(-10000, -10000, -10000);

    public override void _Ready() {
        basket.Visible = false;
        nameLabel.Text = Name;
        for (int i = 0; i < npcNeeds.Length; i++) {
            Need need = (Need)Enum.Parse(typeof(Need), i.ToString());
            int randAmt = 100; //RandRange(90, 100);
            npcNeeds[i] = new NPCNeed(need, randAmt);
        }

        //init scehdule
        for (int i = 0; i < Globals.Instance.GameTime.GetMaxTimeSlots; i++)
            schedule.Add(null);

        //set house
        house.SetNPCOwner(this);

        jobTaskIndex = -1;
        GameEvents.OnTimeIncrease += DecreaseAllNeeds;
        GameEvents.OnTimeIncrease += NextTask;
        NavigationPaths.Instance.OnNavigationMapCreated += SetNavigationMap;

        CallDeferred(MethodName.WaitForNavigationSync);
    }

    #region AI Navigation Movement
    public async void WaitForNavigationSync() {
        SetPhysicsProcess(false);
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
        SetPhysicsProcess(true);

        navMapRID = NavigationPaths.Instance.GetNavigationMapNPC;

        // SetNextTask(fa)
        // NextTask(0);
    }

    public void SetNavigationMap() {
        agent.SetNavigationMap(navMapRID);
    }

    public override void _PhysicsProcess(double delta) {
        //do interaction


        //if moved by something else, i.e. a cart
        if (isRemoteControlled)
            return;

        //if following other character
        if (isFollowing) {
            //UpdatePathfinding();
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


        Vector3 nextLoc = agent.GetNextPathPosition();
        //PrintS(Name, GlobalPosition, agent.TargetPosition);
        Vector3 offset = nextLoc - GlobalPosition;
        Vector3 newVel = offset.Normalized() * speed;

        if (Velocity == pastVel) {
            GlobalPosition = nextLoc;
            return;
        }

        agent.Velocity = newVel;
        pastVel = Velocity;

        MoveAndSlide();

        //rotation
        offset.Y = 0;
        if (GlobalTransform.Origin.IsEqualApprox(GlobalPosition + offset))
            return;
        LookAt(GlobalPosition + offset, Vector3.Up);
    }

    public void Move(Vector3 pos) {
        agent.SetNavigationMap(navMapRID);
        agent.TargetPosition = pos;
    }

    //don't collide with other things and hopefully npcs
    public void OnNavigationAgent3DVelocityComputed(Vector3 safeVel) {
        if (isRemoteControlled)
            return;
        Velocity = safeVel;
        MoveAndSlide();
    }

    public void FollowCharacter(NPC npc) {
        npcToFollow = npc;
        if (isFollowing) return;
        followTimer.Timeout += UpdatePathfinding;
        followTimer.Start();
        isFollowing = true;
    }

    public void UpdatePathfinding() {
        agent.TargetPosition = npcToFollow.GlobalPosition + new Vector3(1, 0, 1);

    }

    public void StopFollow() {
        if (!isFollowing) return;

        npcToFollow = null;
        followTimer.Timeout -= UpdatePathfinding;
        isFollowing = false;

    }

    #endregion
    //.--------------NPC AI-------------------

    public bool IsDoingTask() => isDoingTask;

    #region NextTask
    protected void NextTask(int time) {
        PrintS(Name, timeLeftOnTask, currTask?.Name);
        if (currTask != null) {

            PrintS(Name, "is doing currTaskStep:", taskStep, !currTask.GetIsFinished);
            //currTask.DoTask(this);
            //taskStep++;
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

        //get next task
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

    protected void ClaimTask(Task task) {
        currTask = task;
        currTask.ClaimTask(this);
        timeLeftOnTask = currTask.GetDuration;
        taskStep = 0;
        taskLabel.Text = currTask.GetTaskName;

        if (!isRemoteControlled && !isFollowing)
            Move(currTask.GetTaskPosition());
        else if (isRemoteControlled) //only carts can move right now, so move using a cart
            driveable.Move(currTask.GetTaskPosition());
        isDoingTask = true;
    }

    //finds the next task 
    protected virtual Task FindNextTask(int time) {
        PrintNeeds();
        //need task

        List<Task> closest = FindTasksInAreaPlace();
        PrintS(Name, closest.Count);

        Need lowest = Need.NONE;
        if (!NeedsHighEnough()) {
            lowest = GetLowestNeed();
            npcNeeds[(int)lowest].Amount += 30;
        }

        //goals task
        PrintS(Name, "personality: ", personality.ToString());
        Task bestTask = GetBestTask(closest, lowest);
        if (bestTask != null && bestTask.GetIsPartnerTask) {
            //social task
            NPC partner = FindSocialPartner(bestTask);
            if (bestTask is TalkTask && partner is not null)
                return bestTask;
            else if (bestTask is not TalkTask && partner is null)
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
        PrintS(Name, "is looking for a home task", house.GetPlaceTasks().Count);
        Task homeTask = GetBestTask(house.GetPlaceTasks(), lowest);
        if (homeTask is not null && homeTask.CheckIfCanDoTask(this)) {
            PrintS(Name, "is doing a home task as a last resort");
            return homeTask;
        }

        return null;
    }


    protected virtual Task FindMandatoryTask(int time) {
        //culture task
        Task cultureTask = culture.tasks[time];
        if (cultureTask.GetIsCultureTask && cultureTask.CheckIfCanDoTask(this)) {
            PrintS(Name, "is doing mandatory Culture Task");
            return cultureTask;
        }

        //job task
        //if time is during job time, find job task
        if (job != null && job.IsJobDay() && time >= job.GetStartTime && time < job.GetEndTime) {
            jobTaskIndex++;
            Task jobTask = job.GetJobTask(this);
            if (jobTask != null) {
                PrintS(Name, "is doing job task");
                return jobTask;
            }
        }

        //school task
        //if time is during school time, find school task
        if (school != null && school.IsJobDay() && time >= school.GetStartTime && time < school.GetEndTime) {
            jobTaskIndex++;
            Task schoolTask = school.GetJobTask(this);
            if (schoolTask != null) {
                PrintS(Name, "is doing school task");
                return schoolTask;
            }
        }

        jobTaskIndex = -1;
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
            potentialTasks.Enqueue(task, task.GetTaskScore(GlobalPosition, lowest, personality));

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

    public void Interupt(Task newTask) {
        interruptedTask = currTask;
        if (currTask is SitTask) currTask.FinishTask(this);
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
        PrintS(Name, "place tasks", shapecast.GetCollisionCount());
        if (!shapecast.IsColliding())
            return closest;
        for (int i = 0; i < shapecast.GetCollisionCount(); i++) {
            if (shapecast.GetCollider(i) is Area3D area && area.GetParent() is CommercialSpace commSpace) {
                PrintS(Name, "place task", commSpace.Name, "task count:", commSpace.GetPlaceTasks().Count);
                foreach (Task task in commSpace.GetPlaceTasks()) {
                    if (!task.GetIsJobTask) {
                        PrintS(Name, "place task", task.Name);
                        closest.Add(task);
                    }
                }
            } else if (shapecast.GetCollider(i) is ITaskHolder taskHolder) {
                PrintS(Name, "taskholder", ((Node3D)shapecast.GetCollider(i)).Name, "task count:", taskHolder.GetTasks().Count);
                foreach (Task task in taskHolder.GetTasks()) {
                    if (!task.GetIsJobTask) {
                        PrintS(Name, "place task", task.Name);
                        closest.Add(task);
                    }
                }
            }
        }

        return closest;
    }

    //Finds all the tasks around the npc
    protected NPC FindSocialPartner(Task partnerTask) {
        NPC npcToTalkTo = null;

        float randNum = Randf();
        //Find NPC
        if (randNum > .3 && npcRelationships?.Count != 0)
            foreach (NPC npc in npcRelationships.Keys) {
                if (npc.currTask is not null && !npc.currTask.GetIsInterruptableTask) continue;
                npcToTalkTo = npc;
                break;
            }
        else {
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

    public NPCRelationship GetNPCRelationship(NPC npc) {
        return npcRelationships.ContainsKey(npc) ? npcRelationships[npc] : null;
    }

    #endregion
    //-----------------------Inventory-------------------------------
    #region Inventory
    //---Hands-----
    public bool AreHandsEmpty() {
        return hands.IsEmpty();
    }

    //Adds an specified item to hands
    public void PickUp(ItemR item) {
        hands.PickUp(item);
    }
    public void PickUp(Item item) {
        hands.PickUp(item);
    }

    //Removes the item from hands
    public Item PutDown(bool canDestroyItem) {
        return hands.PutDown(canDestroyItem);
    }

    //---Basket---

    //Checks if the basket is empty
    public bool IsBasketEmpty() {
        return basket.IsEmpty();
    }

    public bool IsBasketFull() {
        return basket.IsFull();
    }

    //Adds an amount of itemR to the basket
    public void AddToBasket(ItemR item, int amt) {
        basket.Visible = true;
        basket.AddToInventory(item, amt);
    }

    //Removes an amount of the specified itemR from the basket
    public void RemoveFromBasket(ItemR item, int amt) {
        basket.RemoveFromInventory(item, amt);
        if (IsBasketEmpty())
            basket.Visible = false;
    }

    //Removes all the items from the basket
    public List<ItemInfo> EmptyBasket() {
        List<ItemInfo> itemInfos = basket.RemoveAllFromInventory();
        basket.Visible = false;
        return itemInfos;
    }

    //Turns off or on collision depending on input
    public void TurnOnCollision(bool canTurnOn) {
        collision.Disabled = canTurnOn;
    }

    //Turns on or off agent navigation depending on input
    public void GiveRemoteControl(bool canGiveRC) {
        isRemoteControlled = canGiveRC;
    }

    public Array<Task> GetTasks() {
        Task[] tasks = [talkTask];
        return new Array<Task>(tasks);
    }

    public void UpdateTaskLabel(string taskMessage) {
        taskLabel.Text += " " + taskMessage;
    }

    public string PrintNextInterruptableTask() {
        return " ct: " + currTask?.Name + " nt: " + nextTask?.Name + " it: " + interruptedTask?.Name;
    }
    #endregion
}
