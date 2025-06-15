using Godot;
using System;

public partial class MainMenuUI : Control {

    [Export] private Control mainMenuPanel;
    [Export] private Control howToPlayPanel;
    [Export] private string mainLevel;

    public void PlayGame() {
        GetTree().ChangeSceneToFile(mainLevel);
    }

    public void QuitGame() {
        GetTree().Quit();
    }

    public void ShowHowToPlayPanel() {
        mainMenuPanel.Visible = false;
        howToPlayPanel.Visible = true;
    }

    public void ShowMainMenuPanel() {
        mainMenuPanel.Visible = true;
        howToPlayPanel.Visible = false;
    }
}
