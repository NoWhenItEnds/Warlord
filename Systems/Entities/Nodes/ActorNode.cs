using Godot;
using Godot.Collections;
using System;
using Warlord.Managers;

namespace Warlord.Entities.Nodes
{
    /// <summary> A node that represents a 'person' within the game world. </summary>
    public partial class ActorNode : CharacterBody3D
    {
        /// <summary> The navigation node used for pathfinding. </summary>
        [ExportGroup("Nodes")]
        [Export] public NavigationAgent3D NavigationAgent { get; private set; }

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
    }
}
