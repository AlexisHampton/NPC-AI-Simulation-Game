using Godot;
using System;

//GameEvents holds all the events in the game to prevent needless dependencies
public class GameEvents {

    public static event Action<int> OnTimeIncrease;
    public static event Action<DayOfTheWeek> OnDayIncrease;

    public static void RaiseTimeIncrease(int gameTime) => OnTimeIncrease?.Invoke(gameTime);
    public static void RaiseDayIncrease(DayOfTheWeek day) => OnDayIncrease?.Invoke(day);
}
