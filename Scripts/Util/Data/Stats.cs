using Godot;

//Stats holds a stat type and value 
[GlobalClass]
public partial class Stats : Resource {
    [Export] public StatType statType;
    [Export] public float value = 1;

    public override string ToString() {
        return statType + " : " + value;
    }
}
