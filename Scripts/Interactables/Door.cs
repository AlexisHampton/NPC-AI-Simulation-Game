using Godot;

//Door is an object that players and soon npcs can interact with to open/close them
public partial class Door : Node3D, IInteractable {

    [Export] private bool isOpen = false;


    [ExportGroup("Meshes")]
    [Export] private Node3D openDoor;
    [Export] private Node3D closedDoor;

    [ExportGroup("Collisions")]
    [Export] private CollisionShape3D openDoorCol;
    [Export] private CollisionShape3D closedDoorCol;

    //Ensures that the complicated process of opening/closing is handled correctly, regardless of changes in the engine
    public override void _Ready() {
        if (isOpen)
            OpenDoor();
        else
            CloseDoor();
    }

    //Opens the door and updates meshes and collisions accordingly
    private void OpenDoor() {
        isOpen = true;
        openDoor.Visible = true;
        openDoorCol.Disabled = false;
        closedDoor.Visible = false;
        closedDoorCol.Disabled = true;
    }

    //Closes the door and updates meshes and collisions accordingly
    private void CloseDoor() {
        isOpen = false;
        openDoor.Visible = false;
        openDoorCol.Disabled = true;
        closedDoor.Visible = true;
        closedDoorCol.Disabled = false;
    }

    //player can close or open the door
    public void Interact(Node3D body) {
        if (isOpen)
            CloseDoor();
        else
            OpenDoor();
    }

}
