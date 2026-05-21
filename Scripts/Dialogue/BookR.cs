using Godot;

//BookR holds the text for a book/letter/piece of interactable text in the world
[GlobalClass]
public partial class BookR : Resource {

    [Export(PropertyHint.MultilineText)] public string Text { get; private set; }
}
