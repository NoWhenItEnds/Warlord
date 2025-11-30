using System.Collections.Generic;
using System.Linq;
using Godot;
using Warlord.Entities.Resources;
using Warlord.Managers;
using Warlord.UI.Components;
using Warlord.Utilities;

namespace Warlord.UI.Windows
{
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

        /// <summary> A reference to the game world's actor manager. </summary>
        private ActorManager _actorManager;


        /// <inheritdoc/>
        public override void _Ready()
        {
            _actorManager = ActorManager.Instance;
            _objectPool = new ObjectPool<ActorCard>(_cardContainer, _actorCardPrefab, 10);

            // TODO - NOT THIS WAY! Have a subscribe to organisation manager.
            foreach (ActorData actor in _actorManager.GetActiveActors())
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


        /// <summary> Free an actor from the game world. </summary>
        /// <param name="actor"> The actor node to remove. </param>
        public void FreeNode(ActorCard actor)
        {
            _objectPool.FreeObject(actor);
            actor.Toggle(null);
            ActorData? data = GetDataFromNode(actor);
            if (data != null)
            {
                _actorMap[data] = null;
            }
        }


        /// <summary> Attempt to get the node associated with a piece of data. </summary>
        /// <param name="data"> The data to search with. </param>
        /// <returns> The associated node, or a null if there isn't any associated with the data. </returns>
        public ActorCard? GetNodeFromData(ActorData data)
        {
            _actorMap.TryGetValue(data, out ActorCard? node);
            return node;
        }


        /// <summary> Attempt to get the data associated with a node. </summary>
        /// <param name="node"> The node to search with. </param>
        /// <returns> The associated data, or a null if there isn't any associated with the node. </returns>
        public ActorData? GetDataFromNode(ActorCard node) => _actorMap.FirstOrDefault(x => x.Value == node).Key ?? null;
    }
}
