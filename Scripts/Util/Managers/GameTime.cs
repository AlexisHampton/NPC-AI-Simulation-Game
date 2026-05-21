using Godot;

public enum DayOfTheWeek { SUN, MON, TUES, WED, THURS, FRI, SAT }

//Handles all the time related to the game
public partial class GameTime : Node, ISaveData {

    [Export] private DirectionalLight3D directionalLight;
    [Export] private float sunRotationSpeed = 0.1f;
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

    //This method is supposed to update the day/night cycle in the game visually
    //disabled for lag
    public override void _Process(double delta) {
        // //directionalLight.RotateX((float)delta * sunRotationSpeed);


        // float rotationAngle = Mathf.DegToRad(gameTime / (float)maxTimeSlots * 360 + 180);
        // directionalLight.Rotation = new Vector3(
        //   Mathf.LerpAngle(directionalLight.Rotation.X, rotationAngle, sunRotationSpeed * (float)delta),
        //   directionalLight.Rotation.Y,
        //   directionalLight.Rotation.Z
        // );
    }

    //Increases the time or day every 30ish seconds
    public void OnGameTimerTimerTimeout() {
        gameTime++;
        if (gameTime > maxTimeSlots - 1)
            NextDay();

        gameTime = Mathf.Clamp(gameTime, 0, maxTimeSlots - 1);

        //directionalLight.RotateX();
        //GD.PrintS("dir", gameTime / (float)maxTimeSlots * 360, directionalLight.Rotation);
        GameEvents.RaiseTimeIncrease(gameTime);
    }


    //Goes to the next day
    public void NextDay() {
        gameTime = 0;
        dayOfTheWeek++;
        dayOfTheWeek = (DayOfTheWeek)Mathf.Wrap((int)dayOfTheWeek, 0, 6);
        GameEvents.RaiseDayIncrease(dayOfTheWeek);
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

    //Loads the saved time data and re-emits the events 
    public void OnLoad(SavedData savedData) {
        SavedDataGameTime sd = (SavedDataGameTime)savedData;
        gameTime = sd.time;
        dayOfTheWeek = sd.day;
        gameTimeTimer.Start(sd.timeLeftOnTimer);
        gameTimeTimer.WaitTime = sd.waitTime;

        GameEvents.RaiseDayIncrease(dayOfTheWeek);
        GameEvents.RaiseTimeIncrease(gameTime);
    }

    //Saves time and day information
    public SavedData OnSave() {
        SavedDataGameTime sd = new SavedDataGameTime();
        sd.time = gameTime;
        sd.day = dayOfTheWeek;
        sd.timeLeftOnTimer = gameTimeTimer.TimeLeft;
        sd.waitTime = gameTimeTimer.WaitTime;

        return sd;
    }
}
