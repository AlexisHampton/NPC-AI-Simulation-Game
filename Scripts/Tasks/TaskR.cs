using Godot;
[GlobalClass]
//A resource that holds task information 
public partial class TaskR : Resource {

    [Export] public string TaskName { get; private set; }
    [Export] public Personality Personality { get; private set; }
    [Export] public int Sparkliness { get; private set; }
    [Export] public int Duration { get; private set; }
    [Export] public bool IsDurationTask { get; private set; }
    [Export] public Need NeedSatisfied { get; private set; }
    [Export] public bool IsJobTask { get; private set; }
    [Export] public bool IsPartnerTask { get; private set; }
    [Export] public bool IsInterruptableTask { get; private set; }
    [Export] public bool IsChildTask { get; private set; }

    public override string ToString() {
        return TaskName + " takes: " + Duration + " time and isDurationTask: " + IsDurationTask + " sparkliness: " + Sparkliness + " and satisfies: " + NeedSatisfied.ToString() + " personality: " + Personality?.personalityDict;
    }

}
