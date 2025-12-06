using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Warlord.Entities.GOAP.Strategies;
using Warlord.Entities.Resources;
using Warlord.Managers;

namespace Warlord.Entities.GOAP
{
    /// <summary> Uses a GOAP implementation to control an entity. The AI brain that controls a unit. </summary>
    public class ActorController
    {
        /// <summary> The current goal the actor is trying to accomplish. </summary>
        public ActorGoal? CurrentGoal { get; private set; } = null;

        /// <summary> The current plan the actor is using to address its current goal. </summary>
        public ActionPlan? CurrentPlan { get; private set; } = null;

        /// <summary> The current action the actor is in the process of doing. </summary>
        public ActorAction? CurrentAction { get; private set; } = null;

        /// <summary> An ordered array of the previous goals the actor tried to accomplish. </summary>
        /// <remarks> [0] is the latest. [^1] is the oldest. </remarks>
        public ActorGoal[] PreviousGoals { get; private set; } = new ActorGoal[10];

        /// <summary> The 'truths' the actor knows. The beliefs it has about the world state. </summary>
        public Dictionary<String, ActorFact> AvailableFacts { get; private set; }

        /// <summary> The goals that the actor will seek to address. </summary>
        public HashSet<ActorGoal> AvailableGoals { get; private set; }

        /// <summary> The potential actions this actor has access to. </summary>
        public HashSet<ActorAction> AvailableActions { get; private set; }

        /// <summary> The goals that the organisation has given the actor. </summary>
        public HashSet<ActorGoal> OrganisationGoals { get; private set; }   // TODO - Implement.


        /// <summary> The entity this controller is responsible for controlling. </summary>
        private readonly ActorData ACTOR;

        /// <summary> A reference to the planner this controller will use. </summary>
        private readonly ActorPlanner PLANNER;


        /// <summary> Uses a GOAP implementation to control an entity. The AI brain that controls a unit. </summary>
        /// <param name="actor"> The entity this controller is responsible for controlling. </param>
        public ActorController(ActorData actor)
        {
            ACTOR = actor;
            PLANNER = new ActorPlanner();

            AvailableActions = new HashSet<ActorAction>();   // Initialise a new set of actions by clearing the current.
            AvailableFacts = new Dictionary<String, ActorFact>();   // Initialise a new set of beliefs by clearing the current.
            FactFactory factory = new FactFactory(ACTOR, AvailableFacts);

            InitialiseBasicPackage(factory);
            InitialiseLocationPackage(factory, LocationManager.Instance.GetData());
            InitialiseBasicGoals();
        }


        /// <summary> Set's the actor's initial facts and actions. </summary>
        /// <param name="factFactory"> A reference to the factor creating these facts. </param>
        private void InitialiseBasicPackage(FactFactory factFactory)
        {
            factFactory.AddFact("Nothing", () => false);  // Always has a belief, even if it never will successfully evaluate.

            factFactory.AddFact("IsHealthy", () => ACTOR.HealthStat.Progress >= 0.9f);
            factFactory.AddFact("IsHurt", () => ACTOR.HealthStat.Progress < 0.5f);
            factFactory.AddFact("IsEntertained", () => ACTOR.EntertainmentStat.Progress >= 0.9f);
            factFactory.AddFact("IsBored", () => ACTOR.EntertainmentStat.Progress < 0.5f);

            AvailableActions.Add(new ActorAction.Builder("Relax", new IdleStrategy(ACTOR, 5f))
                .AddOutcome(AvailableFacts["Nothing"])
                .Build());

            AvailableActions.Add(new ActorAction.Builder("WanderAround", new WanderStrategy(ACTOR))
                .WithCost(() => 10f)    // TODO - Have calculated from actor personality.
                .AddOutcome(AvailableFacts["IsEntertained"])
                .Build());
        }


        /// <summary> Initialise all the facts and actions based upon locations within the game world. </summary>
        /// <param name="factFactory"> A reference to the factor creating these facts. </param>
        /// <param name="locations"> All the locations within the game world. </param>
        private void InitialiseLocationPackage(FactFactory factFactory, LocationData[] locations)
        {
            foreach (LocationData location in locations)
            {
                // Add facts.
                factFactory.AddLocationFact($"At{location.FormattedName}", 1f, location);

                // Add actions.
                AvailableActions.Add(new ActorAction.Builder($"GoTo{location.FormattedName}", new GoToLocationStrategy(ACTOR, location))
                    // TODO - Add cost.
                    .AddOutcome(AvailableFacts[$"At{location.FormattedName}"])
                    .Build());
            }
        }


        /// <summary> Set's the actor's initial actions. </summary>
        private void InitialiseActions()
        {


            /*


            AvailableActions.Add(new ActorAction.Builder("GoToApple")   // TODO - Have harvest apple with a higher cost.
                .WithStrategy(new GoToItemActionStrategy(ACTOR, "apple"))
                .WithCost(10)
                .AddPrecondition(AvailableBeliefs["KnowsApple"])
                .AddOutcome(AvailableBeliefs["AtApple"])
                .Build());

            AvailableActions.Add(new ActorAction.Builder("PickupApple")
                .WithStrategy(new PickupActionStrategy(ACTOR, "apple"))
                .WithCost(0)
                .AddPrecondition(AvailableBeliefs["AtApple"])
                .AddOutcome(AvailableBeliefs["HasApple"])
                .Build());

            AvailableActions.Add(new ActorAction.Builder("EatApple")
                .WithStrategy(new UseInventoryItemActionStrategy(ACTOR, "apple"))
                .WithCost(5)    // This is how which items the agent prefers is encoded. Favorite items have a lower cost.
                .AddPrecondition(AvailableBeliefs["HasApple"])
                .AddOutcome(AvailableBeliefs["IsFed"])
                .Build());*/
        }


        /// <summary> Set's the agent's initial goals relating to basic upkeep. </summary>
        private void InitialiseBasicGoals()
        {
            AvailableGoals = new HashSet<ActorGoal>();

            AvailableGoals.Add(new ActorGoal.Builder("WatchPaintDry", ActorGoal.GoalSource.BASIC)
                .WithPriority(0)
                .WithDesiredOutcome(AvailableFacts["Nothing"])
                .Build());

            AvailableGoals.Add(new ActorGoal.Builder("KeepEntertained", ActorGoal.GoalSource.BASIC)
                .WithPriority(10)
                .WithDesiredOutcome(AvailableFacts["IsEntertained"])
                .Build());
        }


        public void TryAddGoal(String name, Int32 priority, String[] preconditions, String[] outcomes)
        {

        }


        /// <summary> Force a hard reset of the current plan. </summary>
        public void ReevaluatePlan()
        {
            // Remove the current objective to force the planner to reevaluate.
            CurrentAction = null;
            ArchiveCurrentGoal();
        }


        /// <summary> Process the actor's plan. </summary>
        /// <param name="delta"> The time since the previous 'frame' this method was called. </param>
        public void ProcessPlan(Double delta)
        {
            // Update the plan and current action if there is one
            if (CurrentAction == null)
            {
                GD.Print($"{ACTOR.Name} -> Calculating new plan...");
                CalculatePlan();

                if (CurrentPlan != null && CurrentPlan.Actions.Count > 0)
                {
                    CurrentGoal = CurrentPlan.ActorGoal;
                    GD.Print($"{ACTOR.Name} -> Goal: {CurrentGoal.Name} with {CurrentPlan.Actions.Count} actions in plan.");

                    CurrentAction = CurrentPlan.Actions.Pop();
                    GD.Print($"{ACTOR.Name} -> Popped action: {CurrentAction.Name}.");

                    // Verify all precondition effects are true
                    if (CurrentAction.Preconditions.All(b => b.Evaluate()))
                    {
                        CurrentAction.Start();
                    }
                    else
                    {
                        GD.Print($"{ACTOR.Name} -> Goal preconditions not met, clearing current action and goal.");

                        CurrentAction = null;
                        CurrentGoal = null;
                    }
                }
            }


            // If we have a current action, execute it
            if (CurrentPlan != null && CurrentAction != null)
            {
                CurrentAction.Update(delta);

                if (CurrentAction.IsComplete)
                {
                    GD.Print($"{ACTOR.Name} -> Action, {CurrentAction.Name}, complete.");

                    CurrentAction.Stop();
                    CurrentAction = null;

                    if (CurrentPlan.Actions.Count == 0)
                    {
                        GD.Print($"{ACTOR.Name} -> Plan complete!");

                        ArchiveCurrentGoal();
                    }
                }
            }
        }


        /// <summary> Attempt to calculate a new plan. </summary>
        private void CalculatePlan()
        {
            Single priorityLevel = CurrentGoal?.Priority ?? 0;

            HashSet<ActorGoal> goalsToCheck = AvailableGoals;

            // If we have a current goal, we only want to check goals with higher priority.
            if (CurrentGoal != null)
            {
                goalsToCheck = new HashSet<ActorGoal>(AvailableGoals.Where(g => g.Priority > priorityLevel));
            }

            ActionPlan? potentialPlan = PLANNER.BuildPlan(this, goalsToCheck, PreviousGoals[0]);
            if (potentialPlan != null)
            {
                CurrentPlan = potentialPlan;
            }
        }


        /// <summary> Adds the current goal to the array of previous goals. </summary>
        private void ArchiveCurrentGoal()
        {
            ActorGoal[] newValues = new ActorGoal[10];
            newValues[0] = CurrentGoal;
            Array.Copy(PreviousGoals, 0, newValues, 1, PreviousGoals.Length - 1);
            PreviousGoals = newValues;

            CurrentGoal = null;
        }
    }
}
