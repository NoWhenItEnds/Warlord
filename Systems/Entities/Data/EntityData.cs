using System;

namespace Warlord.Entities.Data
{
    /// <summary> The data representing an entity, which is a thing within the game world. </summary>
    public abstract class EntityData
    {
        /// <summary> The common name of the entity. </summary>
        public String Name { get; protected set; } = "SKITTER";
    }
}
