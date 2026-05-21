using Godot;
using System.Collections.Generic;

//Player holds all the player information
public partial class Player : CharacterBody3D, ICharacter {

    [ExportGroup("Shapecasts")]
    [Export] private ShapeCast3D interactionCast;
    [Export] private ShapeCast3D hitCast;
    [Export] private ShapeCast3D npcReactionCast;

    [ExportGroup("Inventory")]
    [Export] private Hands hands;
    [Export] private Basket basket;
    [Export] public InventoryStatic Inventory { get; private set; }

    [ExportGroup("Tasks")]
    [Export] public PlayerTalkTask PlayerTalkTask { get; private set; }

    public bool IsBusy { get; private set; }

    public override void _Ready() {
        GameEvents.OnStopPlayerMovement += HandleMovementStopped;
        GameEvents.OnPlayerDialogue += HandleMovementStopped;
        GameEvents.OnLoadGame += () => HandleMovementStopped(false);
        interactionCast.AddExceptionRid(GetRid());
        hitCast.AddExceptionRid(GetRid());
        npcReactionCast.AddExceptionRid(GetRid());
        basket.Visible = false;
        // npcReactionCast.Enabled = false;
    }

    public override void _Process(double delta) {
        // If press F key, interact with first interactable object
        if (Input.IsActionJustPressed(Constants.INTERACT) && interactionCast.IsColliding() && interactionCast.GetCollider(0) is IInteractable interactable) {
            interactable.Interact(this);
            //GD.PrintS("interacting with", interactionCast.GetCollider(0));
        }    //If press F and have something in hand, add it to inventory 
        else if (Input.IsActionJustPressed(Constants.INTERACT) && !AreHandsEmpty()) {
            Item item = GetItem();
            PutDown(false);
            Inventory.AddToInventory(item, 1);

        }// If press E key, alt interact with first interactable
        else if (Input.IsActionJustPressed(Constants.ALT_INTERACT) && interactionCast.IsColliding() && interactionCast.GetCollider(0) is IAltInteractable altInteractable) {
            altInteractable.AltInteract(this);
            //GD.PrintS("alt interacting with", interactionCast.GetCollider(0));
        }

        if (Input.IsActionJustPressed(Constants.ATTACK_BASE) && hitCast.IsColliding() && hitCast.GetCollider(0) is IHittable hittable) {
            hittable.Hit(this);
            // GD.PrintS("attacking", hitCast.GetCollider(0));

        }
    }


    #region Inventory
    public bool AreHandsEmpty() => hands.IsEmpty();
    public void PickUp(Item item) => hands.PickUp(item);
    public Item PutDown(bool canDestroyItem) => hands.PutDown(canDestroyItem);
    public Item GetItem() => hands.GetItem();

    //---Basket---
    public bool IsBasketEmpty() => basket.IsEmpty();
    public bool IsBasketFull() => basket.IsFull();
    public void AddToBasket(Item item, int amt) => basket.AddToBasket(item, amt);
    public void AddToBasket(Item item) => basket.AddToBasket(item);
    public void RemoveFromBasket(ItemR item, int amt) => basket.RemoveFromBasket(item, amt);
    public List<ItemInfo> EmptyBasket() => basket.EmptyBasket();

    public void TurnInventoryOn(bool isOn) => Inventory.TurnInventoryOn(isOn);

    #endregion

    private void HandleMovementStopped(bool isStopped) {
        if (!isStopped) {
            Input.MouseMode = Input.MouseModeEnum.Captured;
            ProcessMode = ProcessModeEnum.Inherit;
            IsBusy = false;
        } else {
            Input.MouseMode = Input.MouseModeEnum.Visible;
            ProcessMode = ProcessModeEnum.Disabled;
            IsBusy = true;
        }
    }

    //Lets all the npcs near the player, react 
    public void React(NPCReaction npcReaction) {
        npcReactionCast.Enabled = true;
        //GD.PrintS("react player collisiing:", npcReactionCast.IsColliding(), "enabl:", npcReactionCast.Enabled);
        if (!npcReactionCast.IsColliding()) {
            //npcReactionCast.Enabled = false;
            return;
        }
        for (int i = 0; i < npcReactionCast.GetCollisionCount(); i++)
            if (npcReactionCast.GetCollider(i) is NPC npc) {
                npc.React(npcReaction);
                GD.PrintS("npc reacting:", npc.Name);
            }
        // npcReactionCast.Enabled = false;

    }

    #region Save Data
    //Loads player information: position, inventory
    public void OnLoad(SavedDataPlayer savedData) {
        GlobalPosition = savedData.position;

        //instantiate item
        /*
        if (savedData.itemScene != "") {
            Item itemInHand = Utilities.InstantiateItem(savedData.itemScene, Vector3.Zero, this);
        }
        */
    }

    //Saves player information: position, inventory
    public SavedDataPlayer OnSave() {
        SavedDataPlayer savedData = new SavedDataPlayer();
        savedData.position = GlobalPosition;
        savedData.scenePath = SceneFilePath;
        //savedData.itemScene = !AreHandsEmpty() ? GetItem().ItemR.ItemScenePath : "";

        return savedData;
    }
    #endregion


}