using System.Collections.Generic;
using System.Linq;
using Warlord.Entities.Nodes.Locations;
using Warlord.Entities.Resources;
using Warlord.Utilities.Extensions;
using Warlord.Utilities.Singletons;

namespace Warlord.Managers
{
    /// <summary> A manager for all the locations within the game world. </summary>
    public partial class LocationManager : SingletonNode3D<LocationManager>
    {
        /// <summary> The internal mapping between data and its representative node. </summary>
        private Dictionary<LocationData, LocationNode> _locationMap = new Dictionary<LocationData, LocationNode>();


        /// <inheritdoc/>
        public override void _Ready()
        {
            // Fill out the map.
            foreach (LocationNode node in this.GetAllChildComponentsByType<LocationNode>())
            {
                _locationMap.Add(node.Data, node);
            }
        }


        /// <summary> Attempt to get the node associated with a piece of data. </summary>
        /// <param name="data"> The data to search with. </param>
        /// <returns> The associated node, or a null if there isn't any associated with the data. </returns>
        public LocationNode? GetNodeFromData(LocationData data)
        {
            _locationMap.TryGetValue(data, out LocationNode? node);
            return node;
        }


        /// <summary> Attempt to get the data associated with a node. </summary>
        /// <param name="node"> The node to search with. </param>
        /// <returns> The associated data, or a null if there isn't any associated with the node. </returns>
        public LocationData? GetDataFromNode(LocationNode node) => _locationMap.FirstOrDefault(x => x.Value == node).Key ?? null;


        /// <summary> Get all the locations within the game world. </summary>
        /// <returns> An array containing all the locations within the game world. </returns>
        public LocationData[] GetLocationData() => _locationMap.Keys.ToArray();
    }
}
