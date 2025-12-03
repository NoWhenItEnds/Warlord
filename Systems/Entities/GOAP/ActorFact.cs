using Godot;
using System;
using System.Collections.Generic;
using Warlord.Entities.Resources;

namespace Warlord.Entities.GOAP
{
    /// <summary> Construct facts on an industrial scale. </summary>
    public class FactFactory
    {
        /// <summary> A reference to the being who the facts are associated with. </summary>
        private readonly ActorData _actor;

        /// <summary> The array of current facts. </summary>
        private readonly Dictionary<String, ActorFact> _facts;


        /// <summary> Construct facts on an industrial scale. </summary>
        /// <param name="actor"> A reference to the being who the facts are associated with. </param>
        /// <param name="facts"> The array of current facts. </param>
        public FactFactory(ActorData actor, Dictionary<String, ActorFact> facts)
        {
            _actor = actor;
            _facts = facts;
        }


        /// <summary> Adds a new fact with a simple boolean conditional. </summary>
        /// <param name="key"> The name of fact. </param>
        /// <param name="condition"> The function the fact uses to evaluate the nature of the condition. </param>
        public void AddFact(String key, Func<Boolean> condition)
        {
            _facts.Add(key, new ActorFact.Builder(key)
                .WithCondition(condition)
                .Build());
        }


        /// <summary> Add a new positional fact that requires the actor to be in range of an actor. </summary>
        /// <param name="key"> The name of fact. </param>
        /// <param name="distance"> The acceptable distance or range from the location. </param>
        /// <param name="otherActor"> The other actor. </param>
        public void AddActorFact(String key, Single distance, ActorData otherActor)
        {
            _facts.Add(key, new ActorFact.Builder(key)
                .WithCondition(() => InRangeOf(_actor.GetNode(), otherActor.GetNode(), distance))
                .Build());
        }


        /// <summary> Add a new positional fact that requires the actor to be in range of a location. </summary>
        /// <param name="key"> The name of fact. </param>
        /// <param name="distance"> The acceptable distance or range from the location. </param>
        /// <param name="location"> The target location. </param>
        public void AddLocationFact(String key, Single distance, LocationData location)
        {
            _facts.Add(key, new ActorFact.Builder(key)
                .WithCondition(() => InRangeOf(_actor.GetNode(), location.GetNode(), distance))
                .Build());
        }


        /// <summary> Checks whether the given target node is within range of the actor. </summary>
        /// <param name="actorNode"> The actor node. </param>
        /// <param name="otherNode"> The other target node. </param>
        /// <param name="range"> The acceptable range. </param>
        /// <returns> If the actor is within acceptable range of the target location. </returns>
        private Boolean InRangeOf(Node3D? actorNode, Node3D? otherNode, Single range)
        {
            if(actorNode != null && otherNode != null) { return actorNode.GlobalPosition.DistanceTo(otherNode.GlobalPosition) < range; }
            else { return false; }
        }
    }


    /// <summary> A piece of knowledge the actor has about the world. </summary>
    public class ActorFact : IEquatable<ActorFact>
    {
        /// <summary> The identifying name or key of the fact. </summary>
        public String Name { get; private set; }

        /// <summary> The functions the fact uses to evaluate the nature of the condition. </summary>
        private List<Func<Boolean>> _conditions = new List<Func<Boolean>>();


        /// <summary> The identifying name or key of the fact. </summary>
        /// <param name="name"> The identifying name or key of the fact. </param>
        private ActorFact(String name)
        {
            Name = name;
        }


        /// <summary> Calculate the condition to find out if the fact is true. </summary>
        /// <returns> Evaluates the fact to see if it is true or not. </returns>
        public Boolean Evaluate()
        {
            Boolean result = false;

            foreach (Func<Boolean> condition in _conditions)
            {
                result = condition();
                if (!result) { break; }
            }

            return result;
        }


        /// <inheritdoc/>
        public override Int32 GetHashCode() => HashCode.Combine(Name);


        /// <inheritdoc/>
        public override Boolean Equals(Object? obj)
        {
            ActorFact? other = obj as ActorFact;
            return other != null ? Name.Equals(other.Name) : false;
        }


        /// <inheritdoc/>
        public bool Equals(ActorFact? other) => Name.Equals(other?.Name);


        /// <summary> A builder for creating and modifying facts. </summary>
        public class Builder
        {
            /// <summary> The fact the builder is associated with. </summary>
            private readonly ActorFact _fact;


            /// <summary> A builder for creating and modifying facts. </summary>
            /// <param name="name"> The identifying name or key of the fact. </param>
            public Builder(String name)
            {
                _fact = new ActorFact(name);
            }


            /// <summary> Add a condition to the fact. </summary>
            /// <param name="condition"> The delegate used to evaluate the condition. </param>
            public Builder WithCondition(Func<Boolean> condition)
            {
                _fact._conditions.Add(condition);
                return this;
            }


            /// <summary> Build the architected fact. </summary>
            /// <returns> The newly constructed fact. </returns>
            public ActorFact Build()
            {
                return _fact;
            }
        }
    }
}
