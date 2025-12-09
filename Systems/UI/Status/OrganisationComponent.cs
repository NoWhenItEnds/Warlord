using Godot;
using System.Collections.Generic;
using System.Linq;
using Warlord.Managers;
using Warlord.Organisations.Objectives;
using Warlord.Utilities;

namespace Warlord.UI.Status
{
    /// <summary> A window for displaying information about the organisation. </summary>
    public partial class OrganisationComponent : Control
    {
        /// <summary> The parent container for objective cards. </summary>
        [ExportGroup("Nodes")]
        [Export] private VBoxContainer _cardContainer;


        /// <summary> The prefab used for spawning objective cards. </summary>
        [ExportGroup("Resources")]
        [Export] private PackedScene _objectiveCardPrefab;


        /// <summary> The object pool for objective cards. </summary>
        private ObjectPool<ObjectiveCard> _objectPool;

        /// <summary> The internal mapping between objective data and its representative card. </summary>
        private Dictionary<OrganisationObjective, ObjectiveCard> _objectiveMap = new Dictionary<OrganisationObjective, ObjectiveCard>();

        /// <summary> A reference to the game world's organisation manager. </summary>
        private OrganisationManager _organisationManager;


        /// <inheritdoc/>
        public override void _Ready()
        {
            _organisationManager = OrganisationManager.Instance;
            _objectPool = new ObjectPool<ObjectiveCard>(_cardContainer, _objectiveCardPrefab, 10);

            _organisationManager.PlayerController.ObjectivesUpdated += OnObjectivesUpdated;
        }


        private void OnObjectivesUpdated(OrganisationObjective[] objectives)
        {
            // Remove objectives no longer there.
            foreach (OrganisationObjective objective in _objectiveMap.Keys.Except(objectives))
            {
                ObjectiveCard card = _objectiveMap[objective];
                _objectPool.FreeObject(card);
                card.Toggle(null);
                _objectiveMap.Remove(objective);
            }

            // Add new objectives.
            foreach (OrganisationObjective objective in objectives.Except(_objectiveMap.Keys))
            {
                ObjectiveCard card = _objectPool.GetAvailableObject();
                card.Toggle(objective);
                _objectiveMap.Add(objective, card);
            }
        }


        public override void _ExitTree()
        {
            _organisationManager.PlayerController.ObjectivesUpdated -= OnObjectivesUpdated;
        }
    }
}
