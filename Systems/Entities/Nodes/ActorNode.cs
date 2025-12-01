using Godot;
using System;
using Warlord.Managers;

namespace Warlord.Entities.Nodes
{
    /// <summary> A node that represents a 'person' within the game world. </summary>
    public partial class ActorNode : CharacterBody3D
    {
        /// <summary> The navigation node used for pathfinding. </summary>
        [ExportGroup("Nodes")]
        [Export] private NavigationAgent3D _navigationAgent;

        private TimeManager _timeManager;


        /// <inheritdoc/>
        public override void _Ready()
        {
            _timeManager = TimeManager.Instance;
        }


        public void SetDestination(Vector3 globalPosition)
        {
            _navigationAgent.TargetPosition = globalPosition;
        }


        /// <inheritdoc/>
        public override void _PhysicsProcess(Double delta)
        {
            if(!_navigationAgent.IsNavigationFinished())
            {
                Single movementDelta = (Single)_timeManager.GameTimeDelta * 0.1f;   // TODO - The deltas don't match. Physics vs normal one? Also, shouldn't need modifier.
                Vector3 nextPathPosition = _navigationAgent.GetNextPathPosition();
                Vector3 newVelocity = GlobalPosition.DirectionTo(nextPathPosition) * movementDelta;
                GlobalPosition = GlobalPosition.MoveToward(GlobalPosition + newVelocity, movementDelta);
            }
        }
    }
}
