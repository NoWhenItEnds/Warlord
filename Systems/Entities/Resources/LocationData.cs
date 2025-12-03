using Godot;
using Warlord.Entities.Nodes.Locations;
using Warlord.Managers;

namespace Warlord.Entities.Resources
{
    [GlobalClass]
    public partial class LocationData : EntityData
    {
        /// <summary> Get the node that represents the location within the game world. </summary>
        /// <returns> The node, or a null if there isn't currently one. </returns>
        public LocationNode? GetNode() => LocationManager.Instance.GetNodeFromData(this);
    }
}
