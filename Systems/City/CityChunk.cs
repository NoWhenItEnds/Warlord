using Godot;
using System;

namespace Warlord.City
{
    /// <summary> A chunk or region of a larger city. </summary>
    public partial class CityChunk : Node3D
    {
        /// <summary> The class used for navigating along the ground in the chunk. </summary>
        /// <remarks> Meshes and collision shapes need to be childed to this for them to be detected. </remarks>
        [ExportGroup("Nodes")]
        [Export] private NavigationRegion3D _navigationRegion;
    }
}
