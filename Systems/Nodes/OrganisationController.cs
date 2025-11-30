using Godot;
using System.Collections.Generic;
using System.Linq;
using Warlord.Entities.Nodes;
using Warlord.Entities.Resources;
using Warlord.Managers;

namespace Warlord.Nodes
{
    /// <summary> An organisation within the game world that manipulates actors. </summary>
    [GlobalClass]
    public partial class OrganisationController : Node
    {
        /// <summary> All the actors that is organisation has control of. </summary>
        private HashSet<ActorData> _controlledActors = new HashSet<ActorData>();


        /// <summary> A reference to the game's actor manager. </summary>
        private ActorManager _actorManager;


        /// <inheritdoc/>
        public override void _Ready()
        {
            _actorManager = ActorManager.Instance;

            // TODO - NOT LIKE THIS. HAVE SELECTOR. Have spawnner.
            ActorData? skitter = _actorManager.GetDataFromName("Skitter");
            if (skitter != null) { _controlledActors.Add(skitter); }
            ActorData? tattletale = _actorManager.GetDataFromName("Tattletale");
            if (tattletale != null) { _controlledActors.Add(tattletale); }

            ActorNode skitterNode = _actorManager.SpawnNode(skitter, Vector3.Zero);
            ActorNode tattletaleNode = _actorManager.SpawnNode(tattletale, Vector3.Zero);
            skitterNode.SetDestination(new Vector3(10, 0, 10));
        }



        /// <summary> Get all the actors controlled by this organisation. </summary>
        /// <returns> An array of the actors controlled by this organisation. </returns>
        public ActorData[] GetActors() => _controlledActors.ToArray();
    }
}
