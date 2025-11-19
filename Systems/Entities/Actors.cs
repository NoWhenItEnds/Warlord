using Godot;
using System;
using System.Collections.Generic;
using Warlord.Entities.Components.Powers;

namespace Warlord.Entities
{
    public partial class Actors : Node
    {
        public Int32 Strength;
        public Int32 Vigor;
        public Int32 Dexterity;
        public Int32 Intellect;
        public Int32 Perception;
        public Int32 Presence;

        /// <summary> A map of the entity's powers and its rank. </summary>
        public Dictionary<Power, Int32> Powers { get; private set; } = new Dictionary<Power, Int32>();
    }
}
