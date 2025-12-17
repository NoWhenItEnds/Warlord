using System;
using Warlord.Entities.Nodes.Actors;
using Warlord.Entities.Nodes.Locations;
using Warlord.Entities.Resources;
using Warlord.Managers;

namespace Warlord.Entities.GOAP.Strategies
{
    /// <summary> An actor attempts to enter a nearby location. </summary>
    public class EnterLocationStrategy : IActionStrategy
    {
        /// <inheritdoc/>
        public Boolean IsValid => ActorManager.Instance.TryGetNode(ACTOR, out ActorNode? _);  // Only allow if the actor has a node in the game world.

        /// <inheritdoc/>
        public Boolean IsComplete => _isEvaluated;


        /// <summary> A reference to the actor being manipulated. </summary>
        private readonly ActorData ACTOR;

        /// <summary> The location being entered. </summary>
        private readonly LocationData LOCATION;

        /// <summary> The acceptable radius the actor must be within the interactable location of the location. </summary>
        private readonly Single RADIUS;


        /// <summary> Whether the strategy has been evaluated. If so, it needs to be reset. </summary>
        private Boolean _isEvaluated = false;


        /// <summary> An actor attempts to enter a nearby location. </summary>
        /// <param name="actor"> A reference to the actor being manipulated. </param>
        /// <param name="location"> The location being entered. </param>
        /// <param name="radius"> The acceptable radius the actor must be within the interactable location of the location. </param>
        public EnterLocationStrategy(ActorData actor, LocationData location, Single radius = 1f)
        {
            ACTOR = actor;
            LOCATION = location;
            RADIUS = radius;
        }


        /// <inheritdoc/>
        public void Start()
        {
            if (ActorManager.Instance.TryGetNode(ACTOR, out ActorNode? actorNode) &&
                LocationManager.Instance.TryGetNode(LOCATION, out LocationNode? locationNode))
            {
                if (actorNode.GetWorldPosition().DistanceTo(locationNode.GetWorldPosition()) <= RADIUS)
                {
                    ACTOR.SetOccupyingLocation(LOCATION);
                    ActorManager.Instance.FreeNode(actorNode);
                }

                _isEvaluated = true;
            }
        }


        /// <inheritdoc/>
        public void Update(Double delta) { }


        /// <inheritdoc/>
        public void Stop()
        {
            _isEvaluated = false;
        }
    }
}
