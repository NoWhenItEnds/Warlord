using System;
using Warlord.Entities.Nodes.Locations;
using Warlord.Entities.Resources;
using Warlord.Managers;

namespace Warlord.Entities.GOAP.Strategies
{
    /// <summary> An actor attempts to leave a location. </summary>
    public class ExitLocationStrategy : IActionStrategy
    {
        /// <inheritdoc/>
        public Boolean IsValid => ACTOR.OccupyingLocation == LOCATION;

        /// <inheritdoc/>
        public Boolean IsComplete => _isEvaluated;


        /// <summary> A reference to the actor being manipulated. </summary>
        private readonly ActorData ACTOR;

        /// <summary> The location being exited. </summary>
        private readonly LocationData LOCATION;


        /// <summary> Whether the strategy has been evaluated. If so, it needs to be reset. </summary>
        private Boolean _isEvaluated = false;


        /// <summary> An actor attempts to leave a location. </summary>
        /// <param name="actor"> A reference to the actor being manipulated. </param>
        /// <param name="location"> The location being exited. </param>
        public ExitLocationStrategy(ActorData actor, LocationData location)
        {
            ACTOR = actor;
            LOCATION = location;
        }


        /// <inheritdoc/>
        public void Start()
        {
            if (LocationManager.Instance.TryGetNode(LOCATION, out LocationNode? locationNode))
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
