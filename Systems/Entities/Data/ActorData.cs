using System.Collections.Generic;

namespace Warlord.Entities.Data
{
    /// <summary> The data representing a 'person' entity. </summary>
    public class ActorData : EntityData
    {
        public Stat Strength { get; init; } = new Stat("attribute_strength", 0, 0, 10);

        public Stat Dexterity { get; init; } = new Stat("attribute_dexterity", 0, 0, 10);

        public Stat Vigor { get; init; } = new Stat("attribute_vigor", 0, 0, 10);

        public Stat Intellect { get; init; } = new Stat("attribute_intellect", 0, 0, 10);

        public Stat Presence { get; init; } = new Stat("attribute_presence", 0, 0, 10);

        public HashSet<Stat> Skills { get; init; } = new HashSet<Stat>();
    }
}
