using Godot;
using System;

namespace Warlord.Entities.Nodes.Locations
{
    /// <summary> A location node within the game world that can be interacted with. </summary>
    public abstract partial class LocationNode : Node3D
    {
        /// <summary> The area an entity needs to be within to be interacted with. </summary>
        [ExportGroup("Nodes")]
        [Export] private Area3D _interactionArea;


        /// <summary> The common, human-readable, name of the building. </summary>
        [ExportGroup("Settings")]
        [Export] public String BuildingName { get; private set; } = String.Empty;
    }
}
