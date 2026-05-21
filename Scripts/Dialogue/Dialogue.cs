using Godot;
using System;
using System.Collections.Generic;

public partial class Dialogue : Node {
    public string id { get; set; }
    public string text { get; set; }
    public string[] goTo { get; set; }
    public string[] options { get; set; }
    public string func { get; set; }

}
