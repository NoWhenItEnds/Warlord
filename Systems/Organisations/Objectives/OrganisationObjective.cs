using System;
using Warlord.Entities.GOAP;

namespace Warlord.Organisations.Objectives
{
    /// <summary> A goal that the organisation wishes to achieve. </summary>
    public abstract class OrganisationObjective
    {
        /// <summary> The organisation issuing the command. </summary>
        protected ActorGoal.Builder _goalBuilder;


        /// <summary> A goal that the organisation wishes to achieve. </summary>
        /// <param name="goalName"> The name of the goal the objective will generate. </param>
        public OrganisationObjective(String goalName)
        {
            _goalBuilder = new ActorGoal.Builder(goalName, ActorGoal.GoalSource.ORGANISATION);
        }


        public abstract void AddGoal(ActorController controller);
    }
}
