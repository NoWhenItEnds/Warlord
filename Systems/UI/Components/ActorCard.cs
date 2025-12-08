using Godot;
using System;
using Warlord.Entities.Resources;
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


        /// <summary> The data associated with the card. A null indicates that there isn't one. </summary>
        private ActorData? _trackedActor = null;


        /// <summary> A reference to the game world's UI manager. </summary>
        private UIManager _uiManager;


        /// <inheritdoc/>
        public override void _Ready()
        {
            Toggle(null);
            _uiManager = UIManager.Instance;
            GuiInput += OnButtonPressed;
        }


        /// <summary> When the card is pressed. </summary>
        /// <param name="event"> The event that triggered the button. </param>
        private void OnButtonPressed(InputEvent @event)
        {
            if(_trackedActor != null && @event is InputEventMouseButton mouseInput && mouseInput.IsPressed())
            {
                if(mouseInput.ButtonIndex == MouseButton.Left)
                {
                    _uiManager.ToggleActorSelection(_trackedActor);
                }
                else if(mouseInput.ButtonIndex == MouseButton.Right)
                {
                    _uiManager.ToggleActorInformationWindow(_trackedActor);
                }
            }
        }


        /// <summary> Toggle the card's data target. </summary>
        /// <param name="actor"> The actor to select. A null indicates to hide the component. </param>
        public void Toggle(ActorData? actor)
        {
            _trackedActor = actor;
            Visible = actor != null;
        }


        /// <inheritdoc/>
        public override void _Process(Double delta)
        {
            UpdateUI();
        }


        /// <summary> Update all the UI elements. </summary>
        private void UpdateUI()
        {
            if (_trackedActor != null)
            {
                _nameLabel.Text = _trackedActor.Name;
            }
        }


        /// <inheritdoc/>
        public override void _ExitTree()
        {
            GuiInput -= OnButtonPressed;
        }
    }
}
