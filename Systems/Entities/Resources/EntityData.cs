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


        /// <inheritdoc/>
        public override Int32 GetHashCode() => HashCode.Combine(Name);


        /// <inheritdoc/>
        public bool Equals(EntityData? other) => Name.ToLower() == other?.Name.ToLower();
    }
}
