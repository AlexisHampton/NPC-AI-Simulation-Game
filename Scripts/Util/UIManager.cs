using Godot;

//Manages all the HUD UI
//development only
public partial class UIManager : Node {

    [ExportGroup("Menus")]
    [Export] private InventoryUI inventoryUI;

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
        timeScaleLabel.Text = Engine.TimeScale.ToString();
    }

    //Checks for buttons that open UI menus
    // public override void _Input(InputEvent @event) {
    //     //Waits for inventory input ('I' or 'B' key) to turn on/off inventoryPanel
    //     if (@event.IsActionPressed("inventory"))
    //         inventoryUI.TurnMenuOn();


    // }
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

    //restarts the level
    public void RestartLevel() {
        GetTree().ReloadCurrentScene();
    }

    //Moves the player back onto the level
    public void UnstickPlayer() {
        Globals.Instance.Player.GlobalPosition = unstickPlayerPosition;
    }

}
