using Godot;
using Godot.Collections;
using System;
using Warlord.Entities.Nodes;
using Warlord.Entities.Resources;
using Warlord.Managers;

namespace Warlord.Entities.GOAP.Strategies
{
    /// <summary> The actor wanders aimlessly. </summary>
    public class WanderStrategy : IActionStrategy
    {
        /// <inheritdoc/>
        public Boolean IsValid => ActorManager.Instance.GetNodeFromData(ACTOR) != null;  // Only allow if the actor has a node in the game world.

        /// <inheritdoc/>
        public Boolean IsComplete => _actorNode != null && _actorNode.NavigationAgent.IsNavigationFinished();

        /// <summary> A reference to the actor being manipulated. </summary>
        private readonly ActorData ACTOR;

        /// <summary> A reference to the node currently representing the actor in the game world. </summary>
        /// <remarks> A null indicates that there currently isn't one. </remarks>
        private ActorNode? _actorNode = null;


        /// <summary> The actor wanders aimlessly. </summary>
        /// <param name="actor"> A reference to the actor being manipulated. </param>
        public WanderStrategy(ActorData actor)
        {
            ACTOR = actor;
        }


        /// <inheritdoc/>
        public void Start()
        {
            _actorNode = ActorManager.Instance.GetNodeFromData(ACTOR);
            if (_actorNode != null)
            {
                // For some reason, we can't use the map directly, so we use it to get the regions instead, which we can use.
                // TODO - Might be able to manually set the navigation map with a region manager (https://docs.godotengine.org/en/latest/tutorials/navigation/navigation_using_navigationmaps.html).
                Rid navigationMap = _actorNode.GetWorld3D().NavigationMap;
                Array<Rid> regions = NavigationServer3D.MapGetRegions(navigationMap);
                if (regions.Count > 0)
                {
                    Vector3 location = NavigationServer3D.RegionGetRandomPoint(regions[0], 0, true);    // TODO - Else will need random here.
                    _actorNode.NavigationAgent.TargetPosition = location;
                }
            }
        }


        /// <inheritdoc/>
        public void Update(double delta)
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
            if (_actorNode != null)
            {
                _actorNode.NavigationAgent.TargetPosition = _actorNode.GlobalPosition;
                _actorNode = null;
            }
        }
    }
}
