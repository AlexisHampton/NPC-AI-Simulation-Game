using Godot;
using System;
using System.IO;
using Godot.Collections;

using static Godot.GD;

public partial class JSONDialogueParser {

    static Json jsonLoader = new Json();

    public static Dictionary<string, Dialogue> GetDialogue(string dFile) {
        Dictionary<string, string> allData = ParseJSON(dFile);
        return TurnJSonToDialogue(allData);
    }


    private static Dictionary<string, string> ParseJSON(string dFile) {
        if (!File.Exists(dFile))
            return null;

        string data = "";

        //read text
        try {
            data = File.ReadAllText(dFile, System.Text.Encoding.ASCII);
        } catch (Exception e) {
            Print(e.Message);
        }

        //load json
        jsonLoader = new Json();
        Error err = jsonLoader.Parse(data);
        if (err != Error.Ok) {
            Print("JSON Load err", err);
            return null;
        }

        Dictionary<string, string> allData = (Dictionary<string, string>)jsonLoader.Data;
        return allData;
    }

    private static Dictionary<string, Dialogue> TurnJSonToDialogue(Dictionary<string, string> allData) {
        Dictionary<string, Dialogue> dialogueDict = new Dictionary<string, Dialogue>();
        foreach (string data in allData.Keys) {
            allData[data] = allData[data].Replace("\\", "");
            var d = Json.ParseString(allData[data]);
            Dictionary dataDict = (Dictionary)d;

            Dialogue dialogue = new Dialogue();
            dialogue.id = data;
            if (dataDict.ContainsKey("text"))
                dialogue.text = (string)dataDict["text"];
            if (dataDict.ContainsKey("func"))
                dialogue.func = (string)dataDict["func"];
            if (dataDict.ContainsKey("goto"))
                dialogue.goTo = dataDict["goto"].ToString().Split("; ");
            if (dataDict.ContainsKey("options"))
                dialogue.options = dataDict["options"].ToString().Split("; ");

            dialogueDict.Add(data, dialogue);
        }
        return dialogueDict;
    }


}
