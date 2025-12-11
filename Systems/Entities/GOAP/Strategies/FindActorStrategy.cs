using Godot;
using System;
using Warlord.Entities.Nodes.Actors;
using Warlord.Entities.Resources;

namespace Warlord.Entities.GOAP.Strategies
{
    /// <summary> An actor attempts to find the target. </summary>
    public class FindActorStrategy : IActionStrategy
    {
        /// <inheritdoc/>
        public Boolean IsValid => throw new NotImplementedException();

        /// <inheritdoc/>
        public Boolean IsComplete => throw new NotImplementedException();


        /// <summary> A reference to the actor being manipulated. </summary>
        private readonly ActorData ACTOR;

        /// <summary> The strategy's target actor. </summary>
        private readonly ActorData TARGET_ACTOR;

        /// <summary> A reference to the node currently representing the actor in the game world. </summary>
        /// <remarks> A null indicates that there currently isn't one. </remarks>
        private ActorNode? _actorNode = null;

        /// <summary> A reference to the node currently representing the other actor in the game world. </summary>
        /// <remarks> A null indicates that there currently isn't one. </remarks>
        private ActorNode? _targetActorNode = null;


        /// <summary> An actor attempts to find the target. </summary>
        /// <param name="actor"> A reference to the actor being manipulated. </param>
        /// <param name="targetActor"> The strategy's target actor. </param>
        public FindActorStrategy(ActorData actor, ActorData targetActor)
        {
            ACTOR = actor;
            TARGET_ACTOR = targetActor;
        }


        /// <inheritdoc/>
        public void Start()
        {
            throw new NotImplementedException();
        }


        /// <inheritdoc/>
        public void Update(double delta)
        {
            throw new NotImplementedException();
        }


        /// <inheritdoc/>
        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
