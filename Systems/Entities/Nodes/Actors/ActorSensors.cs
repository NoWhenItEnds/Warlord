using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Warlord.Utilities.Extensions;

namespace Warlord.Entities.Nodes.Actors
{
    /// <summary> The parent sensor component that controls an actor's sensors. </summary>
    public partial class ActorSensors : Node3D
    {
        /// <summary> The raycast representing an actor's sight range. </summary>
        [ExportGroup("Nodes")]
        [Export] private Area3D _sightRadius;

        /// <summary> A raycast to use to check if an actor can visually see an object. </summary>
        [Export] private RayCast3D _sightRaycast;


        /// <summary> The array of entity nodes that are potentially visible. </summary>
        private HashSet<IEntityNode> _nearbyEntities = new HashSet<IEntityNode>();

        /// <summary> The array of entities that are presently visible. </summary>
        private HashSet<IEntityNode> _visibleEntities = new HashSet<IEntityNode>();

        /// <summary> The current entity being checked for line of sight. </summary>
        private IEntityNode? _currentEntity = null;


        /// <inheritdoc/>
        public override void _Ready()
        {
            _sightRadius.BodyEntered += OnBodyEntered;
            _sightRadius.BodyExited += OnBodyExited;
        }


        /// <summary> Triggered when a new object enters the sensor's field of view. These are objects that a potentially visible. </summary>
        /// <param name="body"> A reference to the new body. </param>
        private void OnBodyEntered(Node3D body)
        {
            if (body is IEntityNode entity && entity != Owner)
            {
                entity.EntityUpdated += OnEntityUpdated;
            }
        }


        /// <summary> Triggered when a new object leaves the sensor's field of view. These are objects that are no longer visible. </summary>
        /// <param name="body"> A reference to the new body. </param>
        private void OnBodyExited(Node3D body)
        {
            if (body is IEntityNode entity)
            {
                _nearbyEntities.Remove(entity);
                entity.EntityUpdated -= OnEntityUpdated;
            }
        }


        /// <summary> When a tracked entity is updated, queue a new update on the sensor. </summary>
        /// <param name="entity"> The entity that's been updated. </param>
        private void OnEntityUpdated(IEntityNode entity) => _nearbyEntities.Add(entity);


        /// <inheritdoc/>
        public override void _PhysicsProcess(Double delta)
        {
            // Check to see if the 'queued', set up last frame, is within line of sight.
            if (_currentEntity != null)
            {
                if (_sightRaycast.IsColliding() &&
                    _sightRaycast.GetCollider() is IEntityNode entity &&
                    entity == _currentEntity)
                {
                    _visibleEntities.Add(_currentEntity);
                }
                else
                {
                    _visibleEntities.Remove(_currentEntity);
                }
            }

            // If there are still entities to check, queue the next element for the next physics tick.
            if (_nearbyEntities.Count > 0)
            {
                // Pop the first element.
                _currentEntity = _nearbyEntities.First();
                _nearbyEntities.Remove(_currentEntity);

                // Prepare by setting the raycast for the next frame.
                _sightRaycast.SafeLookAt(_currentEntity.GetWorldPosition());
            }
            else
            {
                _currentEntity = null;
            }
        }


        /// <summary> Get all the entities currently visible to the sensor. </summary>
        /// <returns> An array of visible entities. </returns>
        public IEntityNode[] GetVisibleEntities() => _visibleEntities.ToArray();
    }
}
