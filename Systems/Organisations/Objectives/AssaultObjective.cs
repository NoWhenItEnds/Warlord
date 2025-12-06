using System;
using Warlord.Entities.GOAP;
using Warlord.Entities.Resources;

namespace Warlord.Organisations.Objectives
{
    /// <summary> Attack the target location. </summary>
    public class AssaultObjective : OrganisationObjective
    {
        /// <summary> The objective's target. </summary>
        public LocationData Target { get; private set; }


        /// <summary> Attack the target location. </summary>
        /// <param name="target"> The objective's target. </param>
        public AssaultObjective(LocationData target) : base($"assault_{target.FormattedName}")
        {
            Target = target;
        }

        public override void AddGoal(ActorController controller)
        {
            throw new NotImplementedException();
        }
    }
}
