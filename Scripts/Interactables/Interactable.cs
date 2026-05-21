using Godot;
using System;

//Interactable toggles an interaction UI every time an interaction area is entered
public partial class Interactable : Node3D {

    [Export] private Area3D interactionArea;

    public override void _Ready() {
        interactionArea.BodyEntered += HandleBodyEntered;
        interactionArea.BodyExited += HandleBodyExited;
        base._Ready();
    }

    protected void TurnOffInteractionArea(bool isOn) {
        interactionArea.Monitoring = isOn;
    }

    public void HandleBodyEntered(Node3D body) {
        if (body is not Player) return;
        GameEvents.RaiseInteractionAreaEntered(true);
    }

    public void HandleBodyExited(Node3D body) {
        if (body is not Player) return;
        GameEvents.RaiseInteractionAreaEntered(false);
    }
}
