using Godot;
using System;
using System.Collections.Generic;
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
                _nearbyEntities.Add(entity);
            }
        }


        /// <summary> Triggered when a new object leaves the sensor's field of view. These are objects that are no longer visible. </summary>
        /// <param name="body"> A reference to the new body. </param>
        private void OnBodyExited(Node3D body)
        {
            if (body is IEntityNode entity)
            {
                _nearbyEntities.Remove(entity);
            }
        }

        // TODO - ??? CHECK DOESN"T LOOK RIGHT.
        public void ProcessSensor(Double delta)
        {
            foreach (IEntityNode current in _nearbyEntities)
            {
                _sightRaycast.SafeLookAt(current.GetWorldPosition());
                _sightRaycast.ForceRaycastUpdate();

                if (_sightRaycast.IsColliding())
                {
                    if (_sightRaycast.GetCollider() is IEntityNode entity && entity == current)
                    {
                        DebugDraw3D.DrawLineHit(_sightRaycast.GlobalPosition, ToGlobal(_sightRaycast.TargetPosition), _sightRaycast.GetCollisionPoint(), true);
                    }
                }
            }
        }
    }
}
