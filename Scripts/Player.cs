using Godot;
using System;

public partial class Player : CharacterBody3D {

    [Export] public InventoryStatic Inventory { get; private set; }

    [Export] private ShapeCast3D interactionCast;
    [Export] private ShapeCast3D hitCast;

    public override void _Ready() {
        GameEvents.OnPlayerMovementStopped += HandleMovementStopped;
    }

    public override void _Process(double delta) {
        if (Input.IsActionJustPressed("interact") && interactionCast.IsColliding() && interactionCast.GetCollider(0) is IInteractable interactable) {
            interactable.Interact(this);
            GD.PrintS("interacting with", interactionCast.GetCollider(0));
        }

        if (Input.IsActionJustPressed("attack") && hitCast.IsColliding() && hitCast.GetCollider(0) is IHittable hittable) {
            hittable.Hit(this);
            GD.PrintS("attacking", hitCast.GetCollider(0));

        }
    }

    public void TurnInventoryOn(bool isOn) {
        Inventory.TurnInventoryOn(isOn);
    }

    private void HandleMovementStopped(bool canMove) {
        if (canMove) {
            Input.MouseMode = Input.MouseModeEnum.Captured;
            ProcessMode = ProcessModeEnum.Inherit;
        } else {
            Input.MouseMode = Input.MouseModeEnum.Visible;
            ProcessMode = ProcessModeEnum.Disabled;
        }
    }



}
