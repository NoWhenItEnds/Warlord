using Godot;
using System;
using Warlord.Entities.Nodes.Actors;
using Warlord.Entities.Resources;

namespace Warlord.Entities.GOAP.Strategies
{
    /// <summary> An actor attempts to find the target. </summary>
    public class FindEntityStrategy : IActionStrategy
    {
        /// <inheritdoc/>
        public Boolean IsValid => throw new NotImplementedException();

        /// <inheritdoc/>
        public Boolean IsComplete => throw new NotImplementedException();


        /// <summary> A reference to the actor being manipulated. </summary>
        private readonly ActorData ACTOR;

        /// <summary> The strategy's target entity. </summary>
        private readonly EntityData TARGET_ENTITY;

        /// <summary> A reference to the node currently representing the actor in the game world. </summary>
        /// <remarks> A null indicates that there currently isn't one. </remarks>
        private ActorNode? _actorNode = null;


        /// <summary> An actor attempts to find the target. </summary>
        /// <param name="actor"> A reference to the actor being manipulated. </param>
        /// <param name="targetEntity"> The strategy's target entity. </param>
        public FindEntityStrategy(ActorData actor, EntityData targetEntity)
        {
            ACTOR = actor;
            TARGET_ENTITY = targetEntity;
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
