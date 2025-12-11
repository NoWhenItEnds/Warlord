using System;
using Godot;

namespace Warlord.Entities.Resources
{
    /// <summary> The data representing an entity, which is a thing within the game world. </summary>
    public abstract partial class EntityData : Resource, IEquatable<EntityData>
    {
        /// <summary> The common name of the entity. </summary>
        [ExportGroup("Basic")]
        [Export] public String Name { get; protected set; } = String.Empty;


        /// <summary> The name of the entity formatted for storage / lookup. </summary>
        public abstract String FormattedName { get; }


        /// <summary> Get the entity's position within the game world. Either its own, or the location it occupies. </summary>
        /// <param name="position"> The entity's position within the game world. </param>
        /// <returns> Whether the entity has a position that is accessible. </returns>
        public abstract Boolean TryGetWorldPosition(out Vector3 position);


        /// <inheritdoc/>
        public override Int32 GetHashCode() => HashCode.Combine(FormattedName);


        /// <inheritdoc/>
        public bool Equals(EntityData? other) => FormattedName == other?.FormattedName;
    }
}
