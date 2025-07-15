using Godot;
using System;

[GlobalClass]
//InventoryCellUI is holds the UI for a single item in an inventory with a UI component
public partial class InventoryCellUI : Button {


    //necessary UI for each cell
    [Export] protected TextureRect image;
    [Export] protected Label itemAmtLabel;
    [Export] protected Label nameLabel;

    //the image to revert to when the cell is depleted
    private Texture2D defaultImage;

    public override void _Ready() {
        defaultImage = image.Texture;
        itemAmtLabel.Text = "";
    }

    //Resets the cell to default
    public void ResetCell() {
        image.Texture = defaultImage;
        itemAmtLabel.Text = "";
        if (nameLabel is not null)
            nameLabel.Text = "";
    }


    //Updates the UI with a sprite, amount and a name
    public void UpdateUI(Texture2D sprite, int amt, string name = "") {
        image.Texture = sprite;
        itemAmtLabel.Text = amt + "";
        if (nameLabel is not null)
            nameLabel.Text = name;
    }


}
