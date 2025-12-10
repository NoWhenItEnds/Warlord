using Godot;
using System;
using Warlord.Managers;

namespace Warlord.Entities.Nodes.Actors
{
    /// <summary> A node that represents a 'person' within the game world. </summary>
    public partial class ActorNode : CharacterBody3D, IEntityNode
    {
        /// <summary> The navigation node used for pathfinding. </summary>
        [ExportGroup("Nodes")]
        [Export] public NavigationAgent3D NavigationAgent { get; private set; }

        [Export] private ActorSensors _sensors;


        /// <summary> If the node is currently active with an attached data object. </summary>
        public Boolean IsActive => Visible;

        private TimeManager _timeManager;


        /// <inheritdoc/>
        public override void _Ready()
        {
            _timeManager = TimeManager.Instance;
        }


        public void HandleMovement(Vector3 destination, Single movementDelta)
        {
            Vector3 newVelocity = GlobalPosition.DirectionTo(destination) * movementDelta;
            GlobalPosition = GlobalPosition.MoveToward(GlobalPosition + newVelocity, movementDelta);
        }


        /// <inheritdoc/>
        public override void _PhysicsProcess(Double delta)
        {
            // TODO - ???

            if(IsActive)
            {
                _sensors.ProcessSensor(delta);
            }
        }



        /// <inheritdoc/>
        public Vector3 GetWorldPosition() => GlobalPosition;
    }
}
