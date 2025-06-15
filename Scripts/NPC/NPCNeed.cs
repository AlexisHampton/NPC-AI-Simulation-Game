using Godot;
using System;

public enum Need { EAT, HYGIENE, REST, FUN, NONE }

//Holds the NPC's needs
public partial class NPCNeed : Node {


    public Need need;
    private int amount;

    public int Amount {
        get => amount;
        set { amount = Mathf.Clamp(value, 0, 100); }
    }

    public NPCNeed(Need needIn, int amountIn) {
        need = needIn;
        amount = amountIn;
    }
}
