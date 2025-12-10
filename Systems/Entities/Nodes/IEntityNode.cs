using System;
using Godot;

namespace Warlord.Entities.Nodes
{
    /// <summary> Flags a node as representing an entity with a data object in the game world. </summary>
    public interface IEntityNode
    {
        /// <summary> An event that tells listeners this node has changed, and that they need to update inturn. </summary>
        public event Action<IEntityNode> EntityUpdated;

        /// <summary> Get the entity's current position within the game world. </summary>
        public Vector3 GetWorldPosition();
    }
}
