using Godot;
using System;

namespace Warlord.Entities.Nodes.Building
{
    /// <summary> A building node within the game world that can be interacted with. </summary>
    public abstract partial class BuildingNode : Node3D
    {
        /// <summary> The common, human-readable, name of the building. </summary>
        [ExportGroup("Settings")]
        [Export] public String BuildingName { get; private set; } = String.Empty;
    }
}
