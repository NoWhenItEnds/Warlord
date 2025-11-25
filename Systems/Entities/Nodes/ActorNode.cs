using Godot;
using System;

namespace Warlord.Entities.Nodes
{
    /// <summary> A node that represents a 'person' within the game world. </summary>
    public partial class ActorNode : Node3D
    {
        /// <summary> The navigation node used for pathfinding. </summary>
        [ExportGroup("Nodes")]
        [Export] private NavigationAgent3D _navigationAgent;


        public override void _Ready()
        {
        }


        public void SetDestination(Vector3 globalPosition)
        {
            _navigationAgent.TargetPosition = globalPosition;
        }


        public override void _PhysicsProcess(Double delta)
        {
            if(!_navigationAgent.IsNavigationFinished())
            {
                Single movementDelta = (Single)delta * 10f;
                Vector3 nextPathPosition = _navigationAgent.GetNextPathPosition();
                Vector3 newVelocity = GlobalPosition.DirectionTo(nextPathPosition) * movementDelta;
                GlobalPosition = GlobalPosition.MoveToward(GlobalPosition + newVelocity, movementDelta);
            }
        }
    }
}
