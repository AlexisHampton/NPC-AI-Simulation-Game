using Godot;
using System;

//SavedDataNPC stores all of the task and navigation information for an NPC
public partial class SavedDataNPC : SavedData {
    //task data
    [Export] public string currTaskName;
    [Export] public int currTaskStep;
    [Export] public int currTaskIndex;
    [Export] public int timeLeftOnTask;
    [Export] public bool isDoingTask;

    //needs data
    public NPCNeed[] npcNeeds; //cannot be exported bc not a Node

    //navigation
    [Export] public Vector3 targetPosition;

    //Customer component -- might delete later
    [Export] public SavedData savedDataCustomerComp;
    [Export] public bool isCustomer;
    [Export] public bool hasOrderedToday;
}
