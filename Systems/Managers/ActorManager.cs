using System.Collections.Generic;
using Godot;
using Warlord.Entities.Data;
using Warlord.Entities.Nodes;
using Warlord.Utilities;
using Warlord.Utilities.Singletons;

namespace Warlord.Managers
{
    /// <summary> The manager that handles actors within the game world. </summary>
    public partial class ActorManager : SingletonNode3D<ActorManager>
    {
        /// <summary> The prefab for spawning actors. </summary>
        [ExportGroup("Resource")]
        [Export] private PackedScene _actorPrefab;


        /// <summary> The object pool for actor nodes. </summary>
        private ObjectPool<ActorNode> _objectPool;

        /// <summary> The internal mapping between data and its representative node. </summary>
        /// <remarks> A null value indicates that the data is present, but there isn't a node in the game world for it. </remarks>
        private Dictionary<ActorData, ActorNode?> _actorMap = new Dictionary<ActorData, ActorNode?>();


        /// <inheritdoc/>
        public override void _Ready()
        {
            _objectPool = new ObjectPool<ActorNode>(this, _actorPrefab);
        }


        /// <summary> Spawn a new actor node to represent an actor entity. </summary>
        /// <param name="data"> The data object the node will represent. </param>
        /// <param name="globalPosition"> The global position to spawn the node at. </param>
        public void SpawnNode(ActorData data, Vector3 globalPosition)
        {
            if(!_actorMap.TryGetValue(data, out ActorNode? actor))
            {
                actor = _objectPool.GetAvailableObject();
                _actorMap.Add(data, actor);
            }
            actor?.GlobalPosition = globalPosition;
        }


        /// <summary> Free an actor from the game world. </summary>
        /// <param name="actor"> The actor node to remove. </param>
        public void FreeNode(ActorNode actor) => _objectPool.FreeObject(actor);
    }
}
