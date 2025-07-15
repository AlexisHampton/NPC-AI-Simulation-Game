using Godot;
using System;

//Bucket is an item that holds a liquid, but just water for now
//not implemented yet, the gardening system does not require it yet
public partial class Bucket : Item {

    [Export] private int maxAmountOfWater = 5;
    [Export] private int amountOfWater = 0;

    public void FillBucket() {
        amountOfWater = maxAmountOfWater;
    }

    public void UseWater() {
        if (amountOfWater > 0)
            amountOfWater -= 1;
    }

}
