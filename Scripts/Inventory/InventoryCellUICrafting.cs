using Godot;

public partial class InventoryCellUICrafting : InventoryCellUI {
    //Colors for crafting buttons 
    [Export] private Color amtEnoughColor;
    [Export] private Color amtNotEnoughColor;

    //Updates the UI with a sprite, amount, if the amount is enough for crafting, and a name
    public void UpdateUI(Texture2D sprite, string amt, bool isEnough, string name = "") {
        image.Texture = sprite;
        itemAmtLabel.Text = amt;
        if (isEnough)
            itemAmtLabel.AddThemeColorOverride("font_color", amtEnoughColor);
        else
            itemAmtLabel.AddThemeColorOverride("font_color", amtNotEnoughColor);

        if (nameLabel is not null)
            nameLabel.Text = name;
    }
}