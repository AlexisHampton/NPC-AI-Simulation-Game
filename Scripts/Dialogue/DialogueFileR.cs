using Godot;
using System.Collections.Generic;

//DialogueFileR holds a path to a dialogueFile
[GlobalClass]
public partial class DialogueFileR : Resource {
    [Export(PropertyHint.File, "*.json")] public string dialogueFile { get; private set; }

    public Godot.Collections.Dictionary<string, Dialogue> DialogueTree { get; set; }
    public List<string> StartSymbols { get; set; }
}
