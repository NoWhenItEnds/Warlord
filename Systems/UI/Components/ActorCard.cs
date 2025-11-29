using Godot;
using System;
using Warlord.Managers;

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


        /// <summary> A reference to the game world's UI manager. </summary>
        private UIManager _uiManager;

        /// <summary> A reference to the game world's actor manager. </summary>
        private ActorManager _actorManager;


        /// <inheritdoc/>
        public override void _Ready()
        {
            _uiManager = UIManager.Instance;
            _actorManager = ActorManager.Instance;
            ButtonDown += OnButtonPressed;
        }


        /// <summary> When the card is pressed. </summary>
        private void OnButtonPressed()
        {

        }


        /// <inheritdoc/>
        public override void _ExitTree()
        {
            ButtonDown -= OnButtonPressed;
        }
    }
}
