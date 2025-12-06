using System;
using Godot;

namespace Warlord.Entities.Resources
{
    [GlobalClass]
    public partial class LocationData : EntityData
    {
        /// <inheritdoc/>
        public override String FormattedName => String.Format("location_{0}", Name.ToLower().Replace(" ", String.Empty));
    }
}
