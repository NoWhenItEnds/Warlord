using System;
using Godot;
using Warlord.Entities.Resources;

namespace Warlord.Entities.Nodes.Locations
{
    /// <summary> A location node within the game world that can be interacted with. </summary>
    public abstract partial class LocationNode : Node3D, IEntityNode
    {
        /// <summary> The area an entity needs to be within to be interacted with. </summary>
        [ExportGroup("Nodes")]
        [Export] private Area3D _interactionArea;


        /// <summary> The entity data attached to the node. </summary>
        [ExportGroup("Resource")]
        [Export] public LocationData Data { get; private set; }


        /// <inheritdoc/>
        public event Action<IEntityNode> EntityUpdated;


        /// <inheritdoc/>
        public Vector3 GetWorldPosition() => _interactionArea.GlobalPosition;
    }
}
