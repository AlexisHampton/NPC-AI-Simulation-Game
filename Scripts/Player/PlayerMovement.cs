using Godot;
using System;

public enum MovementType { NONE, DASH, DOUBLEJUMP }

public partial class PlayerMovement : Node3D {

    private CharacterBody3D body;

    [ExportGroup("Movement")]
    [Export] private float normalSpeed = 600;
    [Export] private float runSpeed = 1200;
    [Export] private float rotationSpeed = 8f;
    [Export] private float jumpHeight = 3;
    [Export] private float apexDuration = 0.5f;
    [Export] private float fallGravity = 45;
    [Export] private Node3D meshRoot;
    [Export] private Timer dashRunTimer;

    [ExportGroup("Camera")]
    [Export] private Node3D twistPivot;
    [Export] private SpringArm3D pitchPivot;
    [Export] private float mouseSensitivity = 0.5f;
    [Export] private float zoomSensitivity = 1f;
    [Export] private float maxZoom = 11;
    [Export] private float minZoom = 1;

    private float speed;
    private float jumpGravity;
    private float zoom;
    private float twistInput;
    private float pitchInput;
    private Vector3 vel;

    private MovementType movementType = MovementType.NONE;

    //Called at the start and initializes movement variables
    public override void _Ready() {
        body = GetParent<CharacterBody3D>();
        speed = normalSpeed;
        jumpGravity = fallGravity;
        zoom = pitchPivot.SpringLength;
        Input.MouseMode = Input.MouseModeEnum.Captured;
        pitchPivot.AddExcludedObject(body.GetRid());
    }

    //Runs every frame and preforms movement calculation
    public override void _PhysicsProcess(double delta) {
        Misc();
        vel = body.Velocity;
        //gravity
        if (!body.IsOnFloor()) {
            if (vel.Y >= 0)
                vel.Y -= jumpGravity * (float)delta;
            else
                vel.Y -= fallGravity * (float)delta;
        }

        //if time left on timer handle extra movement
        if (dashRunTimer.TimeLeft > 0)
            //and is running
            if (movementType == MovementType.DASH) {
                speed *= 4;
            } else if (movementType == MovementType.DOUBLEJUMP) {
                vel.Y = 2 * jumpHeight / apexDuration;
                jumpGravity = vel.Y / apexDuration;
            }

        //jump
        if (Input.IsActionPressed(Constants.JUMP) && body.IsOnFloor()) {
            vel.Y = 2 * jumpHeight / apexDuration;
            jumpGravity = vel.Y / apexDuration;
            if (dashRunTimer.TimeLeft == 0 && Input.IsActionJustPressed(Constants.JUMP)) {
                dashRunTimer.Start();
                //movementType = MovementType.DOUBLEJUMP;
            }
        }
        //zoom
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        pitchPivot.SpringLength = zoom;

        //run

        if (Input.IsActionPressed(Constants.RUN)) {
            speed = runSpeed;
            if (dashRunTimer.TimeLeft == 0) {
                dashRunTimer.Start();
                movementType = MovementType.DASH;
            }

        } else {
            speed = normalSpeed;
        }

        //movement
        Vector2 input = Input.GetVector(Constants.LEFT, Constants.RIGHT, Constants.UP, Constants.DOWN);
        Vector3 moveDir = (twistPivot.Basis * new Vector3(input.X, 0, input.Y)).Normalized();

        if (moveDir != Vector3.Zero) {
            vel.X = moveDir.X * speed * (float)delta;
            vel.Z = moveDir.Z * speed * (float)delta;
            float targetAngle = Mathf.Atan2(moveDir.X, moveDir.Z);
            meshRoot.Rotation = new Vector3(
                meshRoot.Rotation.X,
                Mathf.LerpAngle(meshRoot.Rotation.Y, targetAngle, (float)delta * rotationSpeed),
                meshRoot.Rotation.Z
            );
        } else {
            vel.X = Mathf.MoveToward(vel.X, 0, speed);
            vel.Z = Mathf.MoveToward(vel.Z, 0, speed);
        }

        body.Velocity = vel;
        body.MoveAndSlide();

        twistInput = pitchInput = 0;
    }

    //Controls camera and zoom input
    public override void _Input(InputEvent @event) {
        if (@event is InputEventMouseMotion mouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured) {
            //rotates camera
            twistInput = -mouseMotion.Relative.X * mouseSensitivity;
            pitchInput = -mouseMotion.Relative.Y * mouseSensitivity;

            twistPivot.RotateY(Mathf.DegToRad(twistInput));
            pitchPivot.RotateX(Mathf.DegToRad(pitchInput));
            pitchPivot.Rotation = new Vector3(
                Mathf.Clamp(pitchPivot.Rotation.X, Mathf.DegToRad(-90), Mathf.DegToRad(90)),
                pitchPivot.Rotation.Y,
                pitchPivot.Rotation.Z
            );
        }
        //zoom input
        if (@event.IsActionPressed(Constants.ZOOMIN))
            zoom -= zoomSensitivity;
        else if (@event.IsActionPressed(Constants.ZOOMOUT))
            zoom += zoomSensitivity;

    }


    public void OnRunAndDashTimerTimeout() {
        if (movementType == MovementType.DASH)
            speed = runSpeed;
        movementType = MovementType.NONE;
    }

    //handles keyboard commands outside of movement
    private void Misc() {
        if (Input.IsActionJustPressed(Constants.UNSTICK)) {
            if (Input.MouseMode == Input.MouseModeEnum.Captured)
                Input.MouseMode = Input.MouseModeEnum.Visible;
            else
                Input.MouseMode = Input.MouseModeEnum.Captured;
        }

    }
}
