using Godot;
using Godot.Collections;

//Globals holds all of the managers. It is the ONLY singleton in the entire game that holds instanced data
//This is to hopefully steer clear of the dependency issue with singletons. 
public partial class Globals : Node {

    public static Globals Instance { get; private set; }

    [ExportGroup("Managers")]
    [Export] public GameTime GameTime { get; private set; }
    [Export] public UIManager UIManager { get; private set; }
    [Export] public Player Player { get; private set; }

    [ExportGroup("Information")]
    [Export] public Dictionary<RelationshipStatus, SocialActionList> AllSocialActions { get; private set; } = new Dictionary<RelationshipStatus, SocialActionList>();
    [Export] public Array<RecipieR> AllCookingRecipies = new();



    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        Instance = this;
    }


}
