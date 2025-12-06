using System;
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
        /// <param name="node"> The associated node, or a null if there isn't any associated with the data. </param>
        /// <returns> Whether there was a node mapped to the given data. </returns>
        public Boolean TryGetNode(LocationData data, out LocationNode? node)
        {
            Boolean result = _locationMap.TryGetValue(data, out LocationNode? value);
            node = value;
            return result;
        }


        /// <summary> Attempt to get the data associated with a node. </summary>
        /// <param name="node"> The node to search with. </param>
        /// <param name="data"> The associated data, or a null if there isn't any associated with the node. </param>
        /// <returns> Whether there was data mapped to the given node. </returns>
        public Boolean TryGetData(LocationNode node, out LocationData? data)
        {
            data = _locationMap.FirstOrDefault(x => x.Value == node).Key ?? null;
            return data != null;
        }


        /// <summary> Get all the locations within the game world. </summary>
        /// <returns> An array containing all the locations within the game world. </returns>
        public LocationData[] GetData() => _locationMap.Keys.ToArray();
    }
}
