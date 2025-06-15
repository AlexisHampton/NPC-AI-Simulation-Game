using Godot;
using Godot.Collections;

[GlobalClass]

//SocialActionList is used to group social actions in the inspector
public partial class SocialActionList : Resource {
    [Export] public Array<SocialAction> allSocialActions { get; private set; } = new Array<SocialAction>();
}
