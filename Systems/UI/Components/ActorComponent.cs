using System;
using Godot;
using Warlord.Entities.Data;
using Warlord.Managers;

namespace Warlord.UI.Components
{
    /// <summary> Displays information about the currently selected actor. </summary>
    public partial class ActorComponent : Control
    {
        /// <summary> The label to show the codename of the actor. </summary>
        [ExportGroup("Nodes")]
        [Export] private RichTextLabel _codenameLabel;


        /// <summary> The currently selected actor. </summary>
        private ActorData? _trackedActor = null;

        /// <summary> A reference to the game world's UI manager. </summary>
        private UIManager _uiManager;


        /// <inheritdoc/>
        public override void _Ready()
        {
            Toggle(null);
            _uiManager = UIManager.Instance;
        }


        /// <summary> Toggle the selector's target. </summary>
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
                _codenameLabel.Text = _trackedActor.Name;
            }
        }
    }
}
