using Godot;
using System;
//Crate is an Item with an amount and a label
public partial class Crate : Item {

    [Export] private Label3D nameLabel;

    private string crateName;
    private int amount;
    private ItemR itemR;

    public string GetCrateName { get => crateName; }
    public int GetItemAmount { get => amount; }
    public ItemR GetItemR { get => itemR; }

    //Sets all the variables and updates thelabel text
    public void MakeCrate(string name, int amt, ItemR ir) {
        amount = amt;
        itemR = ir;
        crateName = name;
        SetNameInLabel(name, amount);
    }


    //updates the label text
    private void SetNameInLabel(string name, int amt) {
        nameLabel.Text = name + ": " + amount;
    }


}
