using System;
using Godot;
using Warlord.Entities.Nodes.Locations;
using Warlord.Managers;

namespace Warlord.Entities.Resources
{
    [GlobalClass]
    public partial class LocationData : EntityData
    {
        /// <inheritdoc/>
        public override String FormattedName => String.Format("location_{0}", Name.ToLower().Replace(" ", String.Empty));


        /// <inheritdoc/>
        public override Boolean TryGetWorldPosition(out Vector3 position)
        {
            Boolean isSuccessful = false;
            position = Vector3.Zero;

            if (LocationManager.Instance.TryGetNode(this, out LocationNode? locationNode) && locationNode != null)
            {
                position = locationNode.GetWorldPosition();
                isSuccessful = true;
            }

            return isSuccessful;
        }
    }
}
