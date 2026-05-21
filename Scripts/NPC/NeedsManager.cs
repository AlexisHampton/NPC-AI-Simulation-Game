using Godot;
using System;

public static partial class NeedsManager {

    private const int LOWEST_NEED_THRESHOLD = 75;
    private const int NEED_DECREASE_AMOUNT = 20;
    //Finds the lowest need
    public static Need GetLowestNeed(NPCNeed[] npcNeeds) {
        Need lowest = Need.NONE;
        int amt = int.MaxValue;

        foreach (NPCNeed n in npcNeeds)
            if (amt > n.Amount) {
                lowest = n.need;
                amt = n.Amount;
            }
        return lowest;

    }
    //checks if the needs are high enough to skip
    public static bool NeedsHighEnough(NPCNeed[] npcNeeds) {
        foreach (NPCNeed npcNeed in npcNeeds)
            if (npcNeed.Amount < LOWEST_NEED_THRESHOLD)
                return false;
        return true;
    }

    //decreases all needs by a specified amount
    public static void DecreaseAllNeeds(NPCNeed[] npcNeeds) {
        foreach (NPCNeed npcNeed in npcNeeds)
            npcNeed.Amount -= NEED_DECREASE_AMOUNT;//1;
    }

    //prints Needs
    public static void PrintNeeds(string name, NPCNeed[] npcNeeds) {
        string needs = name + " ";
        foreach (NPCNeed n in npcNeeds) {
            needs += n.need.ToString() + " " + n.Amount + " ";
        }
        GD.Print(needs);
    }
}
