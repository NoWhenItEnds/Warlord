using System;
using System.Linq;
using Warlord.Entities.GOAP;
using Warlord.Utilities.Exceptions;

namespace Warlord.Organisations.Objectives
{
    /// <summary> A goal that the organisation wishes to achieve. </summary>
    public abstract class OrganisationObjective : IEquatable<OrganisationObjective>
    {
        /// <summary> The name of the generated goal. </summary>
        public String GoalName { get; }

        /// <summary> The organisation issuing the command. </summary>
        protected readonly ActorGoal.Builder GOAL_BUILDER;


        /// <summary> A goal that the organisation wishes to achieve. </summary>
        /// <param name="goalName"> The name of the goal the objective will generate. </param>
        public OrganisationObjective(String goalName)
        {
            GoalName = goalName;
            GOAL_BUILDER = new ActorGoal.Builder(goalName, GoalSource.ORGANISATION);
        }


        /// <summary> Adds the generated goal to an actor. </summary>
        /// <param name="controller"> A reference to the controller to modify. </param>
        /// <param name="priority"> How important the goal current is. </param>
        /// <exception cref="GOAPException"/>
        public abstract void AddGoal(ActorController controller, GoalPriority priority = GoalPriority.LOW);


        public void SetPriority(ActorController controller, GoalPriority priority)
        {
            ActorGoal? goal = controller.AvailableGoals.FirstOrDefault(x => x.Name == GoalName) ?? null;
            if(goal == null)
            {
                throw new GOAPException($"The goal, '{GoalName}', does not exist within {controller.Actor.FormattedName}'s ActionController's goals when attempting to update its priority from an organisation objective. This shouldn't be possible.");
            }

            if(goal.Priority != priority)
            {
                goal.UpdatePriority(priority);
                controller.ReevaluatePlan();
            }
        }


        /// <summary> Try to remove the organisation's goal from the given actor. </summary>
        /// <param name="controller"> A reference to the controller to modify. </param>
        /// <returns> If the goal was removed. </returns>
        public Boolean TryRemoveGoal(ActorController controller)
        {
            Boolean isSuccessful = controller.AvailableGoals.RemoveWhere(x => x.Name == GoalName) > 0;
            if (isSuccessful) { controller.ReevaluatePlan(); }
            return isSuccessful;
        }


        /// <inheritdoc/>
        public override Int32 GetHashCode() => HashCode.Combine(GoalName);


        /// <inheritdoc/>
        public override Boolean Equals(Object? obj)
        {
            OrganisationObjective? other = obj as OrganisationObjective;
            return other != null ? GoalName.Equals(other.GoalName) : false;
        }


        /// <inheritdoc/>
        public Boolean Equals(OrganisationObjective? other) => GoalName.Equals(other?.GoalName);
    }
}
