using Godot;
using System;
using System.Linq;
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


        /// <inheritdoc/>
        public event Action<IEntityNode> EntityUpdated;


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
            EntityUpdated?.Invoke(this);
        }


        /// <summary> Checks if the given entity is currently visible by the actor. </summary>
        /// <param name="entity"> The entity to check for. </param>
        /// <returns> Is the given entity visible? </returns>
        public Boolean IsEntityVisible(IEntityNode entity) => _sensors.GetVisibleEntities().FirstOrDefault(x => x == entity) != null;


        /// <inheritdoc/>
        public Vector3 GetWorldPosition() => GlobalPosition;
    }
}
