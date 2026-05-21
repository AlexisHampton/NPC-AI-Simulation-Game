using Godot;

//Stove is used by players and npcs to cook
//The player seeds a crafting UI they can interact with
//The NPC portion is not completely implemented yet 
public partial class CookBook : StaticBody3D, IInteractable {

    [Export] private CraftingUI craftingUI;

    public override void _Ready() {
        craftingUI.Visible = false;
    }

    public void Interact(Node3D body) {
        if (body is Player player) {
            //show cooking UI
            craftingUI.Visible = true;
            craftingUI.LoadAllRecipies(player);
        }
    }
}
