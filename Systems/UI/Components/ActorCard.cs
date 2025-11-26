using Godot;
using System;

namespace Warlord.UI.Components
{
    /// <summary> A card on the UI representing a controlled actor. </summary>
    public partial class ActorCard : Button
    {
        /// <summary> The texture displaying the actor's portrait. </summary>
        [ExportGroup("Nodes")]
        [Export] private TextureRect _texture;

        /// <summary> The label displaying the actor's name / title. </summary>
        [Export]private RichTextLabel _nameLabel;
    }
}
