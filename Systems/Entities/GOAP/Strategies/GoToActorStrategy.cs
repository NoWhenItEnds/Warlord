using System;
using Godot;
using Warlord.Entities.Nodes.Actors;
using Warlord.Entities.Resources;
using Warlord.Managers;

namespace Warlord.Entities.GOAP.Strategies
{
    /// <summary> An actor moves itself to the given actor's position. </summary>
    public class GoToActorStrategy : IActionStrategy
    {
        /// <inheritdoc/>
        public Boolean IsValid => ActorManager.Instance.TryGetNode(ACTOR, out ActorNode? _);  // Only allow if the actor has a node in the game world.

        /// <inheritdoc/>
        public Boolean IsComplete => _actorNode != null && _actorNode.NavigationAgent.IsNavigationFinished();

        /// <summary> A reference to the actor being manipulated. </summary>
        private readonly ActorData ACTOR;

        /// <summary> The strategy's target actor. </summary>
        private readonly ActorData TARGET_ACTOR;

        /// <summary> A reference to the node currently representing the actor in the game world. </summary>
        /// <remarks> A null indicates that there currently isn't one. </remarks>
        private ActorNode? _actorNode = null;


        /// <summary> An actor moves itself to the given actor's position. </summary>
        /// <param name="actor"> A reference to the actor being manipulated. </param>
        /// <param name="targetActor"> The strategy's target actor. </param>
        public GoToActorStrategy(ActorData actor, ActorData targetActor)
        {
            ACTOR = actor;
            TARGET_ACTOR = targetActor;
        }


        /// <inheritdoc/>
        public void Start()
        {
            if (ActorManager.Instance.TryGetNode(ACTOR, out _actorNode) &&
                TARGET_ACTOR.TryGetWorldPosition(out Vector3 targetPosition))
            {
                _actorNode.NavigationAgent.TargetPosition = targetPosition;
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
            if (_actorNode != null)
            {
                _actorNode.NavigationAgent.TargetPosition = _actorNode.GlobalPosition;
                _actorNode = null;
            }
        }
    }
}
