using System;
using Warlord.Entities.GOAP;
using Warlord.Entities.Resources;
using Warlord.Utilities.Exceptions;

namespace Warlord.Organisations.Objectives
{
    /// <summary> Defend the target location. This also represents the default idle state. </summary>
    public class GarrisonObjective : OrganisationObjective
    {
        /// <summary> The objective's target. </summary>
        public LocationData Target { get; private set; }


        /// <summary> Defend the target location. </summary>
        /// <param name="target"> The objective's target. </param>
        public GarrisonObjective(LocationData target) : base($"garrison_{target.FormattedName}")
        {
            Target = target;
        }

        public override void AddGoal(ActorController controller)
        {
            String factName = $"at_{Target.FormattedName}";
            if (controller.AvailableFacts.TryGetValue(factName, out ActorFact? atFact))
            {
                ActorGoal goal = _goalBuilder
                    .WithPriority(0f)   // TODO - Set somehow.
                    .WithDesiredOutcome(atFact)
                    .Build();
                controller.AvailableGoals.Add(goal);
            }
            else
            {
                throw new GOAPException($"The fact, '{factName}', does not exist within {controller.Actor.FormattedName}'s ActionController's available facts.");
            }
        }
    }
}
