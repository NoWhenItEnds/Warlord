using System.Collections.Generic;
using Godot;
using Warlord.Entities.Resources;
using Warlord.Managers;
using Warlord.UI.Components;
using Warlord.Utilities;

namespace Warlord.UI.Windows
{
    /// <summary> The window to display the controlled actor cards. </summary>
    public partial class ActorCardWindow : Control
    {
        /// <summary> The parent container for actor cards. </summary>
        [ExportGroup("Nodes")]
        [Export] private HBoxContainer _cardContainer;


        /// <summary> The prefab used for spawning actor cards. </summary>
        [ExportGroup("Resources")]
        [Export] private PackedScene _actorCardPrefab;


        /// <summary> The object pool for actor cards. </summary>
        private ObjectPool<ActorCard> _objectPool;

        /// <summary> The internal mapping between data and its representative card. </summary>
        private Dictionary<ActorData, ActorCard> _actorMap = new Dictionary<ActorData, ActorCard>();

        /// <summary> A reference to the game world's organisation manager. </summary>
        private OrganisationManager _organisationManager;


        /// <inheritdoc/>
        public override void _Ready()
        {
            _organisationManager = OrganisationManager.Instance;
            _objectPool = new ObjectPool<ActorCard>(_cardContainer, _actorCardPrefab, 10);

            // TODO - NOT THIS WAY! Have a subscribe to organisation manager.
            foreach (ActorData actor in _organisationManager.PlayerController.GetActors())
            {
                SpawnNode(actor);
            }
        }


        /// <summary> Spawn a new actor card to represent an actor entity. </summary>
        /// <param name="data"> The data object the node will represent. </param>
        /// <returns> A reference to the spawned card. </returns>
        public ActorCard SpawnNode(ActorData data)
        {
            if (!_actorMap.TryGetValue(data, out ActorCard? actor))
            {
                actor = _objectPool.GetAvailableObject();
                _actorMap.Add(data, actor);
            }
            actor.Toggle(data);
            return actor;
        }
    }
}
