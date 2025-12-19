using System;
using Warlord.Entities.Nodes.Locations;
using Warlord.Entities.Resources;
using Warlord.Managers;
using Warlord.Utilities.Exceptions;

namespace Warlord.Entities.GOAP.Strategies
{
    /// <summary> An actor attempts to leave a location. </summary>
    public class ExitLocationStrategy : IActionStrategy
    {
        /// <inheritdoc/>
        public Boolean IsValid => ACTOR.OccupyingLocation != null;

        /// <inheritdoc/>
        public Boolean IsComplete => _isEvaluated;


        /// <summary> A reference to the actor being manipulated. </summary>
        private readonly ActorData ACTOR;


        /// <summary> Whether the strategy has been evaluated. If so, it needs to be reset. </summary>
        private Boolean _isEvaluated = false;


        /// <summary> An actor attempts to leave a location. </summary>
        /// <param name="actor"> A reference to the actor being manipulated. </param>
        public ExitLocationStrategy(ActorData actor)
        {
            ACTOR = actor;
        }


        /// <inheritdoc/>
        public void Start()
        {
            if (ACTOR.OccupyingLocation == null)
            {
                throw new GOAPException($"{ACTOR.Name}'s occupying location is NULL, despite the strategy evaluating it as not. Something has gone very wrong.");
            }

            if (LocationManager.Instance.TryGetNode(ACTOR.OccupyingLocation, out LocationNode? locationNode))
            {
                ACTOR.SetOccupyingLocation(null);
                ActorManager.Instance.SpawnNode(ACTOR, locationNode.GetWorldPosition());
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
