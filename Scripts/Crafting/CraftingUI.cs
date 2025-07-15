using System.Collections.Immutable;
using Godot;
using Godot.Collections;

//The UI for all crafting systems
public partial class CraftingUI : Panel {

    [Export] private Array<Button> craftingButtons = new();
    [Export] private Label descriptionLabel;
    [Export] private Array<InventoryCellUICrafting> ingredientUIs = new();
    [Export] private LineEdit craftingAmountInput;

    private RecipieR currRecipie;
    private Player player;
    private int craftingAmount = 1;
    private bool hasEnoughIngredients = false;

    public override void _Ready() {
        craftingAmountInput.TextChanged += ModifyCraftingAmount;
    }

    //All known crafting recipies are loaded into the UI
    public void LoadAllRecipies(Player playerIn) {
        craftingAmount = 1;
        craftingAmountInput.Text = craftingAmount + "";

        player = playerIn;
        GameEvents.RaisePlayerMovementStopped(false);
        Array<RecipieR> recipies = Globals.Instance.AllCookingRecipies;
        //makes as many buttons as there are recipies
        for (int i = 0; i < Globals.Instance.AllCookingRecipies.Count; i++) {
            craftingButtons[i].Text = recipies[i].RecipieName;
            craftingButtons[i].Visible = true;
        }
    }

    //Loads the UI for the current recipie
    public void LoadRecipieUI(int button) {
        RecipieR recipie = Globals.Instance.AllCookingRecipies[button];

        currRecipie = recipie;
        descriptionLabel.Text = recipie.Description;
        UpdateIngredientItemAmounts();
    }

    //Modifies the amount to be crafted, between 1 and 100, via the + and - buttons
    public void ModifyCraftingAmount(int amt) {
        craftingAmount += amt;
        craftingAmount = Mathf.Clamp(craftingAmount, 1, 100);
        craftingAmountInput.Text = craftingAmount + "";
        UpdateIngredientItemAmounts();

    }

    //Modifies the crafting amount from a LineEdit node 
    public void ModifyCraftingAmount(string amt) {
        if (!int.TryParse(amt, out int newCraftingAmount)) return;

        craftingAmount = Mathf.Clamp(newCraftingAmount, 1, 100);
        craftingAmountInput.Text = craftingAmount + "";
        UpdateIngredientItemAmounts();
    }

    //updates the amounts on the ingredient inventoryUICells 
    private void UpdateIngredientItemAmounts() {
        int uiCounter = 0;
        foreach (IngredientR ing in currRecipie.Ingredients.Keys) {
            int playerIngAmt = player.Inventory.GetItemAmount(ing);
            int recipieIngAmt = currRecipie.Ingredients[ing] * craftingAmount;
            hasEnoughIngredients = playerIngAmt >= recipieIngAmt;

            string itemAmount = playerIngAmt + "/" + recipieIngAmt;

            ingredientUIs[uiCounter].UpdateUI(ing.Sprite, itemAmount, hasEnoughIngredients, ing.ItemName);
            uiCounter++;
        }
    }

    //Make the recipie
    public void Craft() {
        //check if player has ingredients
        if (!hasEnoughIngredients) return;
        foreach (IngredientR ing in currRecipie.Ingredients.Keys) {
            if (!player.Inventory.HasItem(ing)) {
                GD.PrintS("Cannot make recipie. Don't have: ", ing.ItemName);
                return;
            }
        }
        //remove ingredients from inventory
        foreach (IngredientR ing in currRecipie.Ingredients.Keys) {
            player.Inventory.RemoveFromInventory(ing, craftingAmount);
        }
        for (int i = 0; i < craftingAmount; i++) {
            //craft recipe
            Item completedItem = currRecipie.completedItem.Instantiate<Item>();
            player.Inventory.AddChild(completedItem);
            completedItem.Position = Vector3.Zero;

            completedItem.DisableItem(true);

            //add it to their inventory
            player.Inventory.AddToInventory(completedItem, 1);
            GD.PrintS("Successfully crafted:", completedItem.ItemR.ItemName);
        }
        //then make player know
        UpdateIngredientItemAmounts();

    }


    //hides the crafting ui and lets the player move
    public void HidePanel() {
        Visible = false;
        GameEvents.RaisePlayerMovementStopped(true);
    }

}
