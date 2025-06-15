using Godot;
using Godot.Collections;
using System.Collections.Generic;

//Commerical Space is a Place that NPCs can hang out in that is not their House
public partial class CommercialSpace : Place {
    [Export] protected Array<ItemR> itemsToSell = new Array<ItemR>();
    [Export] protected int startHours = 2;
    [Export] protected int endHours = 10;

    [Export] protected int maxCapacity = 3;
    [Export] protected Array<NPC> employees = new();
    [Export] protected Array<DayOfTheWeek> openDays = new Array<DayOfTheWeek>();


    protected bool isOpen = false;
    protected int currCapacity = 0;

    protected List<NPC> line = new List<NPC>();

    public Array<ItemR> GetItemsToSell { get => itemsToSell; }
    public bool GetIsOpen { get => isOpen; }

    public override void _Ready() {
        base._Ready();
        GameEvents.OnTimeIncrease += IsClosingTime;
        GameEvents.OnTimeIncrease += IsOpeningTime;

        nameLabel.Text = Name;
    }


    //Opens the store or not based on input
    public void OpenStore(bool open) {
        isOpen = open;
        GD.PrintS(Name, "is open: ", isOpen);
    }


    public bool IsStoreOpenDay() {
        return openDays.Contains(Globals.Instance.GameTime.GetDayOfTheWeek);
    }
    //Checks if it's two hours before closing time and closes the store
    public void IsClosingTime(int gameTime) {
        if (!IsStoreOpenDay()) return;
        if (gameTime == endHours - 2) {
            OpenStore(false);
        }
    }

    //checks if it's opening time and opens the store
    public void IsOpeningTime(int gameTime) {
        if (!IsStoreOpenDay()) return;
        if (gameTime == startHours)
            OpenStore(true);
    }



    //Checks if a specified npc is an employee
    public bool IsEmployee(NPC npc) {
        return employees.Contains(npc);
    }

    public bool IsStoreFull() {
        return currCapacity >= maxCapacity;
    }

    public bool AddCapacity() {
        if (IsStoreFull())
            return false;
        currCapacity++;
        GD.PrintS(Name, "currCap:", currCapacity);
        return IsStoreFull();
    }

    public void DecreaseCapacity() {
        currCapacity--;
        GD.PrintS(Name, "currCap:", currCapacity);
    }

    //returns if the line is empty
    public bool IsLineEmpty() {
        return line.Count == 0;
    }

    //Adds a specified npc to the line
    public void AddCustomerToLine(NPC npc) {
        line.Add(npc);
    }

    //Returns if the npc is still on line
    public bool IsCustomerStillOnLine(NPC npc) {
        return line.Contains(npc);
    }

}
