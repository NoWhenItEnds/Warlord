using System;
using Godot;
using Warlord.Entities.Nodes;
using Warlord.Entities.Nodes.Locations;
using Warlord.Entities.Resources;
using Warlord.Managers;

namespace Warlord.Entities.GOAP.Strategies
{
    /// <summary> An actor moves itself to the given location. </summary>
    public partial class GoToLocationStrategy : IActionStrategy
    {
        /// <inheritdoc/>
        public Boolean IsValid => ActorManager.Instance.GetNodeFromData(ACTOR) != null;  // Only allow if the actor has a node in the game world.

        /// <inheritdoc/>
        public Boolean IsComplete => _actorNode != null && _actorNode.NavigationAgent.IsNavigationFinished();

        /// <summary> A reference to the actor being manipulated. </summary>
        private readonly ActorData ACTOR;

        /// <summary> The strategy's target location. </summary>
        private readonly LocationData LOCATION;

        /// <summary> A reference to the node currently representing the actor in the game world. </summary>
        /// <remarks> A null indicates that there currently isn't one. </remarks>
        private ActorNode? _actorNode = null;

        /// <summary> A reference to the node currently representing the location in the game world. </summary>
        /// <remarks> A null indicates that there currently isn't one. </remarks>
        private LocationNode? _locationNode = null;


        /// <summary> An actor moves itself to the given location. </summary>
        /// <param name="actor"> A reference to the actor being manipulated. </param>
        /// <param name="location"> The strategy's target location. </param>
        public GoToLocationStrategy(ActorData actor, LocationData location)
        {
            ACTOR = actor;
            LOCATION = location;
        }


        /// <inheritdoc/>
        public void Start()
        {
            _actorNode = ActorManager.Instance.GetNodeFromData(ACTOR);
            _locationNode = LocationManager.Instance.GetNodeFromData(LOCATION);
            if (_actorNode != null && _locationNode != null)
            {
                _actorNode.NavigationAgent.TargetPosition = _locationNode.GetInteractionPosition();
            }
        }


        /// <inheritdoc/>
        public void Update(Double delta)
        {
            if (_actorNode != null && !_actorNode.NavigationAgent.IsNavigationFinished())
            {
                Vector3 nextPosition = _actorNode.NavigationAgent.GetNextPathPosition();
                _actorNode.HandleMovement(nextPosition, (Single)delta); // TODO - use gametime delta instead. Should be handled in node?
            }
        }


        /// <inheritdoc/>
        public void Stop()
        {
            if(_actorNode != null)
            {
                _actorNode.NavigationAgent.TargetPosition = _actorNode.GlobalPosition;
                _actorNode = null;
            }

            _locationNode = null;
        }
    }
}
