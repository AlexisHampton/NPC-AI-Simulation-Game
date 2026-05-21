using Godot;
using Godot.Collections;


//SaveLoaderManager implements saving and loading functionality for the entire game
public partial class SaveLoaderManager : Node {

    [Export] private string defaultPath = "user://savedGame.tres";
    [Export] private Player player;

    public override void _Ready() {
        GameEvents.OnLoadGame += LoadGame;
        GameEvents.OnSaveGame += SaveGame;
    }

    //Saves all the nodes that are marked for saving to a readable godot resource file
    public void SaveGame() {
        GD.PrintS("saving...");
        SavedGame savedGame = new SavedGame();

        Array<SavedData> savedData = new();

        //save static information
        savedGame.playerData = player.OnSave();
        savedGame.gameTimeData = Globals.Instance.GameTime.OnSave();

        //Saves all dynamic data that is marked for saving
        foreach (Node node in GetTree().GetNodesInGroup(Constants.SAVEABLE)) {
            //type check since groups are prone to human error
            if (node is not ISaveData saveable) continue;
            SavedData sd = saveable.OnSave();
            if (saveable is NPC npc) {
                //saves the index of the npc meant to be saved, if an npc is not in the list, their saved data won't be loaded, but will be saved
                sd.Index = Globals.Instance.SavedNPCs.IndexOf(npc);
            }
            GD.PrintS("saved: ", node.Name);
            savedData.Add(sd);
        }

        GD.PrintS("saved data size: ", savedData.Count);
        savedGame.savedData = savedData;
        ResourceSaver.Save(savedGame, defaultPath);
    }

    public void LoadGame() {
        SavedGame savedGame = ResourceLoader.Load(defaultPath) as SavedGame;

        //load the statics
        player.OnLoad(savedGame.playerData);
        Globals.Instance.GameTime.OnLoad(savedGame.gameTimeData);

        //load dynamics
        foreach (SavedData sd in savedGame.savedData) {
            //reinstantiate them if index is -1
            if (sd.Index == -1) {
                //reinstantiate
                GD.PrintS("reinstantiate", sd.scenePath);
            }
            //load their data 
            else {
                if (sd is SavedDataNPC) {
                    NPC npc = Globals.Instance.SavedNPCs[sd.Index];
                    // npc.OnLoad(sd);
                }
            }
        }

    }
}
