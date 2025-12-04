using Warlord.Entities.GOAP;
using Warlord.Entities.Resources;

namespace Warlord.Organisations.Objectives
{
    /// <summary> Defend the target location. This also represents the default idle state. </summary>
    public class GarrisonObjective : OrganisationObjective
    {
        /// <summary> The objective's target. </summary>
        public LocationData Target { get; private set; }


        /// <summary> Attack the target location. </summary>
        /// <param name="organisation"> The organisation issuing the command. </param>
        /// <param name="target"> The objective's target. </param>
        public GarrisonObjective(OrganisationController organisation, LocationData target) : base(organisation)
        {
            Target = target;
        }

        public override void AddGoal(ActorController controller)
        {
            throw new System.NotImplementedException();
        }
    }
}
