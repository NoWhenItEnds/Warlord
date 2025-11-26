using System.Collections.Generic;

namespace Warlord.Entities.Data
{
    /// <summary> The data representing a 'person' entity. </summary>
    public class ActorData : EntityData
    {
        public Stat Strength { get; init; } = new Stat("attribute_strength", 0, 0, 10);

        public HashSet<Stat> Skills { get; init; } = new HashSet<Stat>();
    }
}
