using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Warlord.Entities.Nodes;
using Warlord.Entities.Resources;
using Warlord.Utilities;
using Warlord.Utilities.Extensions;
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

        /// <summary> The preloaded premade actor data. </summary>
        private HashSet<ActorData> _actorData = new HashSet<ActorData>();

        /// <summary> The internal mapping between data and its representative node. </summary>
        /// <remarks> A null value indicates that the data is present, but there isn't a node in the game world for it. </remarks>
        private Dictionary<ActorData, ActorNode?> _actorMap = new Dictionary<ActorData, ActorNode?>();


        /// <inheritdoc/>
        public override void _Ready()
        {
            _objectPool = new ObjectPool<ActorNode>(this, _actorPrefab);

            List<String> actorPaths = new List<String>(FileExtensions.GetResources("res://data/actors", [".tres"]));
            //actorPaths.AddRange(FileExtensions.GetResources("user://data/actors", [".tres"]));
            foreach (String path in actorPaths)
            {
                _actorData.Add(GD.Load<ActorData>(path));
            }

            // TODO - NOT THIS.
            ActorData skitterData = _actorData.First(x => x.Name == "Skitter");
            ActorNode node0 = SpawnNode(skitterData, new Vector3(0, 0, 0));
            ActorData tattletaleData = _actorData.First(x => x.Name == "Tattletale");
            ActorNode node1 = SpawnNode(tattletaleData, new Vector3(0, 0, 0));
            node0.SetDestination(new Vector3(10, 0, 10));
        }


        /// <summary> Spawn a new actor node to represent an actor entity. </summary>
        /// <param name="data"> The data object the node will represent. </param>
        /// <param name="globalPosition"> The global position to spawn the node at. </param>
        /// <returns> A reference to the spawned node. </returns>
        public ActorNode SpawnNode(ActorData data, Vector3 globalPosition)
        {
            if(!_actorMap.TryGetValue(data, out ActorNode? actor))
            {
                actor = _objectPool.GetAvailableObject();
                _actorMap.Add(data, actor);
            }
            actor?.GlobalPosition = globalPosition;
            return actor;
        }


        /// <summary> Free an actor from the game world. </summary>
        /// <param name="actor"> The actor node to remove. </param>
        public void FreeNode(ActorNode actor)
        {
            _objectPool.FreeObject(actor);
            ActorData? data = GetDataFromNode(actor);
            if(data != null)
            {
                _actorMap[data] = null;
            }
        }


        /// <summary> Attempt to get the node associated with a piece of data. </summary>
        /// <param name="data"> The data to search with. </param>
        /// <returns> The associated node, or a null if there isn't any associated with the data. </returns>
        public ActorNode? GetNodeFromData(ActorData data)
        {
            _actorMap.TryGetValue(data, out ActorNode? node);
            return node;
        }


        /// <summary> Attempt to get the data associated with a node. </summary>
        /// <param name="node"> The node to search with. </param>
        /// <returns> The associated data, or a null if there isn't any associated with the node. </returns>
        public ActorData? GetDataFromNode(ActorNode node) => _actorMap.FirstOrDefault(x => x.Value == node).Key ?? null;



        public ActorData[] GetActiveActors()    // TODO - Move this to organisation controller.
        {
            return _actorMap.Where(x => x.Value != null).Select(x => x.Key).ToArray();
        }
    }
}
