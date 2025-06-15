using Godot;

public enum DayOfTheWeek { SUN, MON, TUES, WED, THURS, FRI, SAT }

//Handles all the time related to the game
public partial class GameTime : Node {

    [Export] private int maxTimeSlots = 48;
    [Export] private Timer gameTimeTimer;
    [Export] private DayOfTheWeek dayOfTheWeek;

    private int gameTime;

    public int GetMaxTimeSlots => maxTimeSlots;
    public DayOfTheWeek GetDayOfTheWeek => dayOfTheWeek;

    public override void _Ready() {
        GameEvents.RaiseDayIncrease(dayOfTheWeek);
        GameEvents.RaiseTimeIncrease(gameTime);
    }

    //Increases the time or day every 30ish seconds
    public void OnGameTimerTimerTimeout() {
        gameTime++;
        if (gameTime > maxTimeSlots - 1) {
            gameTime = 0;
            dayOfTheWeek++;
            dayOfTheWeek = (DayOfTheWeek)Mathf.Wrap((int)dayOfTheWeek, 0, 6);
            GameEvents.RaiseDayIncrease(dayOfTheWeek);
        }
        gameTime = Mathf.Clamp(gameTime, 0, maxTimeSlots - 1);
        GameEvents.RaiseTimeIncrease(gameTime);
    }


    //converts gameTime to military time
    public override string ToString() {
        string timeStr = "";
        timeStr += (gameTime / 2) + " : ";
        if (gameTime % 2 == 0)
            timeStr += "00";
        else
            timeStr += "30";
        return timeStr;
    }
}
