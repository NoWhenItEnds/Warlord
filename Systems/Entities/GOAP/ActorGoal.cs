using System;
using System.Collections.Generic;

namespace Warlord.Entities.GOAP
{
    /// <summary> A goal that the actor desires and will use a chain of actions to complete. </summary>
    /// <remarks> Under the hood this is really just a series of desired states / facts. </remarks>
    public class ActorGoal : IEquatable<ActorGoal>
    {
        /// <summary> The name or key identifying the goal. </summary>
        public String Name { get; }

        /// <summary> The importance of the goal the actor. More important goals will be tackled before less. </summary>
        public Single Priority { get; private set; } = 1f;

        /// <summary> The desired state of the world for the goal to be considered complete. All actions should go towards addressing these outcomes. </summary>
        public HashSet<ActorFact> DesiredOutcomes { get; } = new();


        /// <summary> A goal that the actor desires and will use a chain of actions to complete. </summary>
        /// <param name="name"> The name or key identifying the goal. </param>
        private ActorGoal(String name)
        {
            Name = name;
        }


        /// <inheritdoc/>
        public override Int32 GetHashCode() => HashCode.Combine(Name);


        /// <inheritdoc/>
        public override Boolean Equals(Object? obj)
        {
            ActorGoal? other = obj as ActorGoal;
            return other != null ? Name.Equals(other.Name) : false;
        }


        /// <inheritdoc/>
        public bool Equals(ActorGoal? other) => Name.Equals(other?.Name);


        /// <summary> A helpful builder that allows for easy construction of actor goals. </summary>
        public class Builder
        {
            /// <summary> A reference to the goal being constructed. </summary>
            private readonly ActorGoal _goal;


            /// <summary> A helpful builder that allows for easy construction of actor goals. </summary>
            /// <param name="name"> The name or key identifying the goal. </param>
            public Builder(String name)
            {
                _goal = new ActorGoal(name);
            }


            /// <summary> Sets the goal's priority. </summary>
            /// <param name="priority"> The importance of the goal the actor. More important goals will be tackled before less. </param>
            public Builder WithPriority(Single priority)
            {
                _goal.Priority = priority;
                return this;
            }


            /// <summary> Adds a desired outcome to the goal. </summary>
            /// <param name="outcome"> The desired state of the world for the goal to be considered complete. </param>
            public Builder WithDesiredOutcome(ActorFact outcome)
            {
                _goal.DesiredOutcomes.Add(outcome);
                return this;
            }


            /// <summary> Adds a desired outcome to the goal. </summary>
            /// <param name="outcomes"> The desired states of the world for the goal to be considered complete. </param>
            public Builder WithDesiredOutcome(ActorFact[] outcomes)
            {
                foreach (ActorFact outcome in outcomes)
                {
                    _goal.DesiredOutcomes.Add(outcome);
                }
                return this;
            }


            /// <summary> Build the architected goal. </summary>
            /// <returns> The newly constructed goal. </returns>
            public ActorGoal Build()
            {
                return _goal;
            }
        }
    }
}
