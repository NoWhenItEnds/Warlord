using System;
using Godot;
using Godot.Collections;

namespace Warlord.Entities.Resources
{
    /// <summary> The data representing a 'person' entity. </summary>
    [GlobalClass]
    public partial class ActorData : EntityData
    {
        [ExportGroup("Stats")]
        [ExportSubgroup("Attributes")]
        [Export] public Stat Strength { get; private set; } = new Stat("attribute_strength", 1, 0, 10);

        [Export] public Stat Dexterity { get; private set; } = new Stat("attribute_dexterity", 1, 0, 10);

        [Export] public Stat Vigor { get; private set; } = new Stat("attribute_vigor", 1, 0, 10);

        [Export] public Stat Intellect { get; private set; } = new Stat("attribute_intellect", 1, 0, 10);

        [Export] public Stat Presence { get; private set; } = new Stat("attribute_presence", 1, 0, 10);


        [ExportSubgroup("Skills")]
        [Export] public Array<Stat> Skills { get; private set; } = new Array<Stat>();


        /// <summary> The data representing a 'person' entity. </summary>
        public ActorData() : base() { }


        /// <summary> Reconstruct an actor from json data. </summary>
        /// <param name="json"> The raw json string. </param>
        /// <returns> The constructed data object. </returns>
        public static ActorData FromJson(String json)
        {
            return new ActorData(); // TODO - Implement.
        }
    }
}
