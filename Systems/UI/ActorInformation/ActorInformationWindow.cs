using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;
using Warlord.Entities.GOAP;
using Warlord.Entities.Resources;
using Warlord.Managers;

namespace Warlord.UI.ActorInformation
{
    /// <summary> A popup window showing information about a selected actor. </summary>
    public partial class ActorInformationWindow : Control
    {
        /// <summary> The name of the currently selected actor. </summary>
        [ExportGroup("Nodes")]
        [ExportSubgroup("Header")]
        [Export] private RichTextLabel _actorNameLabel;

        /// <summary> The button to close the information window. </summary>
        [Export] private Button _closeButton;

        /// <summary> The radar chart used to represent an actor's attribute values. </summary>
        [ExportSubgroup("Left Column")]
        [Export] private ActorAttributeChart _attributeChart;

        /// <summary> A label to use for showing debug information. </summary>
        [ExportSubgroup("Right Column")]
        [Export] private RichTextLabel _debugLabel; // TODO - Replace with actual ui.


        /// <summary> The current actor to show information about. A null indicates that there isn't one. </summary>
        private ActorData? _selectedActor = null;

        /// <summary> The GOAP controller associated with the selected actor. </summary>
        private ActorController? _selectedActorController = null;

        /// <summary> A reference to the game world's actor manager. </summary>
        private ActorManager _actorManager;


        /// <inheritdoc/>
        public override void _Ready()
        {
            _actorManager = ActorManager.Instance;

            Toggle(null);
            _closeButton.ButtonDown += () => Toggle(null);
        }


        /// <summary> Toggle the window's data target. </summary>
        /// <param name="actor"> The actor to select. A null indicates to hide the component. </param>
        public void Toggle(ActorData? actor)
        {
            _selectedActor = actor;
            Visible = actor != null;

            if(actor != null)
            {
                _selectedActorController = _actorManager.GetController(actor);
            }
            else
            {
                _selectedActorController = null;
            }
        }


        /// <inheritdoc/>
        public override void _Process(Double delta)
        {
            UpdateUI();
        }


        /// <summary> Update all the UI elements. </summary>
        private void UpdateUI()
        {
            if (_selectedActor != null && _selectedActorController != null)
            {
                _actorNameLabel.Text = _selectedActor.Name;

                // Left column.
                _attributeChart.SetAttributes(_selectedActor.Strength.Percent, _selectedActor.Dexterity.Percent, _selectedActor.Vigor.Percent, _selectedActor.Intellect.Percent, _selectedActor.Presence.Percent);

                // Populate the debug window with GOAP information.
                StringBuilder debugText = new StringBuilder();
                debugText.AppendLine("--- GOALS ---");
                foreach (ActorGoal goal in _selectedActorController.AvailableGoals.OrderByDescending(x => x.Priority))
                {
                    debugText.AppendLine($"{goal.Name}: {goal.Priority.ToString()}");
                }
                debugText.AppendLine("--- FACTS ---");
                foreach (KeyValuePair<String, ActorFact> fact in _selectedActorController.AvailableFacts)
                {
                    Color evaluateColour = fact.Value.Evaluate() ? Colors.Green : Colors.Red;
                    debugText.AppendLine($"[color=#{evaluateColour.ToHtml()}]{fact.Key}[/color]");
                }
                _debugLabel.Text = debugText.ToString();
            }
        }


        /// <inheritdoc/>
        public override void _ExitTree()
        {
            _closeButton.ButtonDown -= () => Toggle(null);
        }
    }
}
