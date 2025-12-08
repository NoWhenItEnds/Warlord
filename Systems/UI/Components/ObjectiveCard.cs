using Godot;
using Warlord.Managers;
using Warlord.Organisations.Objectives;

namespace Warlord.UI.Components
{
    /// <summary> A UI element that represents an organisational objective. </summary>
    public partial class ObjectiveCard : Control
    {
        /// <summary> A label to show the objective's name. </summary>
        [ExportGroup("Nodes")]
        [Export] private RichTextLabel _nameLabel;

        /// <summary> The node to select the objective's priority. </summary>
        [Export] private SpinBox _prioritySelector;

        /// <summary> The button to remove the objective. </summary>
        [Export] private Button _deleteButton;


        /// <summary> A reference to the game world's organisation manager. </summary>
        private OrganisationManager _organisationManager;

        /// <summary> The objective this card represents. </summary>
        private OrganisationObjective? _objective = null;


        /// <inheritdoc/>
        public override void _Ready()
        {
            _organisationManager = OrganisationManager.Instance;
            _deleteButton.ButtonDown += OnDeleteButton;
        }


        private void OnDeleteButton()
        {
            if(_objective != null)
            {
                _organisationManager.PlayerController.RemoveObjective(_objective);
            }
        }


        /// <summary> Toggle the card's data target. </summary>
        /// <param name="objective"> The objective to represent. A null indicates to hide the component. </param>
        public void Toggle(OrganisationObjective? objective)
        {
            _objective = objective;
            Visible = objective != null;

            if(objective != null)
            {
                _nameLabel.Text = objective.GoalName;
            }
        }


        /// <inheritdoc/>
        public override void _ExitTree()
        {
            _deleteButton.ButtonDown -= OnDeleteButton;
        }
    }
}
