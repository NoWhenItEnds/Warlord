using Warlord.Entities.Resources;

namespace Warlord.Organisations.Objectives
{
    /// <summary> Attack the target location. </summary>
    public class AssaultObjective : OrganisationObjective
    {
        /// <summary> The objective's target. </summary>
        public LocationData Target { get; private set; }


        /// <summary> Attack the target location. </summary>
        /// <param name="organisation"> The organisation issuing the command. </param>
        /// <param name="target"> The objective's target. </param>
        public AssaultObjective(OrganisationController organisation, LocationData target) : base(organisation)
        {
            Target = target;
        }
    }
}
