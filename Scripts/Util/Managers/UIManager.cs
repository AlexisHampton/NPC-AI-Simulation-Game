using Godot;

//Manages all the HUD UI
//development only
public partial class UIManager : Node {

    [ExportGroup("OnScreenUI")]
    [Export] private Control interactionUI;

    [ExportGroup("Menus")]
    [Export] private Control pauseMenu;

    [ExportGroup("Time")]
    [Export] private Label timeLabel;
    [Export] private Label dayLabel;

    [ExportGroup("TimeScale")]
    [Export] private Label timeScaleLabel;

    [ExportGroup("Player")]
    [Export] private Vector3 unstickPlayerPosition;

    //subscribes to events
    public override void _Ready() {
        GameEvents.OnTimeIncrease += UpdateGameTimeUI;
        GameEvents.OnDayIncrease += UpdateGameDayUI;
        GameEvents.OnPauseGame += TurnPauseOn;
        GameEvents.OnInteractionAreaEntered += TurnInteractionUIOn;
        timeScaleLabel.Text = Engine.TimeScale.ToString();
        TurnPauseOn(false);
        TurnInteractionUIOn(false);

    }

    public void TurnPauseOn(bool isOn) {
        pauseMenu.Visible = isOn;
    }

    public void TurnInteractionUIOn(bool isOn) {
        interactionUI.Visible = isOn;
    }

    //updates the time in military time
    public void UpdateGameTimeUI(int gameTime) {
        timeLabel.Text = Globals.Instance.GameTime.ToString();
    }

    //Updates the day of the week
    public void UpdateGameDayUI(DayOfTheWeek day) {
        dayLabel.Text = day.ToString();
    }

    //Makes the game run faster or slower
    public void UpdateTimeScaleUI(bool canIncrease) {
        if (canIncrease) Engine.TimeScale += 1;
        else Engine.TimeScale -= 1;

        timeScaleLabel.Text = Engine.TimeScale.ToString();

    }

    public void ResumeGame() => TurnPauseOn(false);

    //restarts the level
    public static void RestartLevel() => Globals.Instance.RestartLevel();

    //Moves the player back onto the level
    public void UnstickPlayer() => Globals.Instance.UnstickPlayer();
    public void QuitGame() => Globals.Instance.Quit();


    public void SaveGame() => GameEvents.RaiseSaveGame();
    public void LoadGame() {
        GameEvents.RaiseLoadGame();
        ResumeGame();
    }



}
