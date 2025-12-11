using System;
using Godot;
using Godot.Collections;
using Warlord.Entities.Nodes.Actors;
using Warlord.Entities.Nodes.Locations;
using Warlord.Managers;
using Warlord.Utilities;

namespace Warlord.Entities.Resources
{
    /// <summary> The data representing a 'person' entity. </summary>
    [GlobalClass]
    public partial class ActorData : EntityData
    {
        /// <inheritdoc/>
        public override String FormattedName => String.Format("actor_{0}", Name.ToLower().Replace(" ", String.Empty));


        [ExportGroup("Stats")]
        [ExportSubgroup("Attributes")]
        [Export] public Stat Strength { get; private set; } = new Stat("attribute_strength", 1, 0, 10);

        [Export] public Stat Dexterity { get; private set; } = new Stat("attribute_dexterity", 1, 0, 10);

        [Export] public Stat Vigor { get; private set; } = new Stat("attribute_vigor", 1, 0, 10);

        [Export] public Stat Intellect { get; private set; } = new Stat("attribute_intellect", 1, 0, 10);

        [Export] public Stat Presence { get; private set; } = new Stat("attribute_presence", 1, 0, 10);


        [ExportSubgroup("Skills")]
        [Export] public Array<Stat> Skills { get; private set; } = new Array<Stat>();


        /// <summary> The actor's physical stamina. </summary>
        public DerivedStat StaminaStat { get; private set; }

        /// <summary> How entertained / satisfied the actor is. </summary>
        public DerivedStat EntertainmentStat { get; private set; }

        /// <summary> The location the actor is currently within. A null indicates that they aren't and are instead in the game world. </summary>
        public LocationData? OccupyingLocation { get; private set; } = null;


        /// <summary> The data representing a 'person' entity. </summary>
        public ActorData() : base()
        {
            StaminaStat = new DerivedStat(() => 0, () => Vigor.CurrentValue + 3);
            EntertainmentStat = new DerivedStat(() => 0, () => 10, 5);  // TODO - Start at max.
        }


        /// <summary> Reconstruct an actor from json data. </summary>
        /// <param name="json"> The raw json string. </param>
        /// <returns> The constructed data object. </returns>
        public static ActorData FromJson(String json)
        {
            return new ActorData(); // TODO - Implement.
        }


        /// <inheritdoc/>
        public override Boolean TryGetWorldPosition(out Vector3 position)
        {
            Boolean isSuccessful = false;
            position = Vector3.Zero;

            if (OccupyingLocation != null)
            {
                if (LocationManager.Instance.TryGetNode(OccupyingLocation, out LocationNode? locationNode) && locationNode != null)
                {
                    position = locationNode.GetWorldPosition();
                    isSuccessful = true;
                }
            }
            else
            {
                if (ActorManager.Instance.TryGetNode(this, out ActorNode? actorNode) && actorNode != null)
                {
                    position = actorNode.GetWorldPosition();
                    isSuccessful = true;
                }
            }

            return isSuccessful;
        }
    }
}
