using Godot;
using System;

public partial class PlayerMovement : Node3D {

    private CharacterBody3D body;

    [ExportGroup("Movment")]
    [Export] private float normalSpeed = 600;
    [Export] private float runSpeed = 1200;
    [Export] private float rotationSpeed = 8;
    [Export] private float jumpHeight = 2;
    [Export] private float apexDuration = 0.5f;
    [Export] private float fallGravity = 45;
    [Export] private Node3D meshRoot;

    [ExportGroup("Camera")]
    [Export] private Node3D twistPivot;
    [Export] private SpringArm3D pitchPivot;
    [Export] private float mouseSensitivity = 0.5f;
    [Export] private float zoomSensitivity = 1;
    [Export] private float minZoom = 1;
    [Export] private float maxZoom = 11;

    private float zoom;
    private float jumpGravity;
    private float speed;
    private float twistInput;
    private float pitchInput;
    private Vector3 vel;
    private bool isInteracting = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        body = GetParent<CharacterBody3D>();
        zoom = pitchPivot.SpringLength;
        speed = normalSpeed;
        jumpGravity = fallGravity;
        pitchPivot.AddExcludedObject(body.GetRid());
        Input.MouseMode = Input.MouseModeEnum.Captured;

    }

    #region Movement and Mouse Input
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
        Misc();

        vel = body.Velocity;
        //gravity
        if (!body.IsOnFloor()) {
            if (vel.Y >= 0)
                vel.Y -= jumpGravity * (float)delta;
            else
                vel.Y -= fallGravity * (float)delta;
        }

        //jump
        if (Input.IsActionPressed("jump") && body.IsOnFloor()) {
            vel.Y = 2 * jumpHeight / apexDuration;
            jumpGravity = vel.Y / apexDuration;
        }

        //zoom
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        pitchPivot.SpringLength = Mathf.MoveToward(pitchPivot.SpringLength, zoom, 10 * (float)delta);

        //run
        if (Input.IsActionPressed("run")) {
            speed = runSpeed;
        } else {
            speed = normalSpeed;
        }

        //Movement
        Vector2 input = Input.GetVector("left", "right", "up", "down");
        Vector3 moveDir = (twistPivot.Basis * new Vector3(input.X, 0, input.Y)).Normalized();

        if (moveDir != Vector3.Zero) {
            vel.X = moveDir.X * speed * (float)delta;
            vel.Z = moveDir.Z * speed * (float)delta;

            float targetAngle = Mathf.Atan2(moveDir.X, moveDir.Z) - Rotation.Y;
            meshRoot.Rotation = new Vector3(
                meshRoot.Rotation.X,
                Mathf.LerpAngle(meshRoot.Rotation.Y, targetAngle, rotationSpeed * (float)delta),
                meshRoot.Rotation.Z
            );
        } else {
            vel.X = Mathf.MoveToward(moveDir.X, 0, speed);
            vel.Z = Mathf.MoveToward(moveDir.Z, 0, speed);
        }

        body.Velocity = vel;

        body.MoveAndSlide();

        twistInput = pitchInput = 0;

    }

    private void Misc() {
        if (Input.IsActionJustPressed("quit"))
            GetTree().Quit();
        if (Input.IsActionJustPressed("alt")) {
            if (Input.MouseMode == Input.MouseModeEnum.Captured)
                Input.MouseMode = Input.MouseModeEnum.Visible;
            else
                Input.MouseMode = Input.MouseModeEnum.Captured;
        }

    }

    public override void _Input(InputEvent @event) {
        if (@event is InputEventMouseMotion mouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured) {
            twistInput = -mouseMotion.Relative.X * mouseSensitivity;
            pitchInput = -mouseMotion.Relative.Y * mouseSensitivity;

            twistPivot.RotateY(Mathf.DegToRad(twistInput));
            pitchPivot.RotateX(Mathf.DegToRad(pitchInput));

            pitchPivot.Rotation = new Vector3(
                Mathf.Clamp(pitchPivot.Rotation.X, Mathf.DegToRad(-90), Mathf.DegToRad(30)),
                pitchPivot.Rotation.Y,
                pitchPivot.Rotation.Z
            );
        }

        if (@event.IsActionPressed("zoomIn"))
            zoom -= zoomSensitivity;
        else if (@event.IsActionPressed("zoomOut"))
            zoom += zoomSensitivity;
    }

    #endregion



}
