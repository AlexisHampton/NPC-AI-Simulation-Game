using Godot;
using System;

//SavedDataGameTime saves the current time in the game
public partial class SavedDataGameTime : SavedData {
    [Export] public int time;
    [Export] public DayOfTheWeek day;
    [Export] public double waitTime;
    [Export] public double timeLeftOnTimer;
}
