using Godot;
using System;

public partial class TraderJob : Job {

    [Export] private Driveable traderCart;

    public Driveable GetTraderCart { get => traderCart; }


}
