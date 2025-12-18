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
        /// <summary> The entity this controller is responsible for controlling. </summary>
        public ActorData Actor { get; init; }

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


        /// <summary> A reference to the planner this controller will use. </summary>
        private readonly ActorPlanner PLANNER;


        /// <summary> Uses a GOAP implementation to control an entity. The AI brain that controls a unit. </summary>
        /// <param name="actor"> The entity this controller is responsible for controlling. </param>
        public ActorController(ActorData actor)
        {
            Actor = actor;
            PLANNER = new ActorPlanner();

            AvailableActions = new HashSet<ActorAction>();   // Initialise a new set of actions by clearing the current.
            AvailableFacts = new Dictionary<String, ActorFact>();   // Initialise a new set of beliefs by clearing the current.
            FactFactory factory = new FactFactory(Actor, AvailableFacts);

            InitialiseBasicPackage(factory);
            InitialiseLocationPackage(factory, LocationManager.Instance.GetData());
            InitialiseActorPackage(factory, ActorManager.Instance.GetData());
            InitialiseBasicGoals();
        }


        /// <summary> Set's the actor's initial facts and actions. </summary>
        /// <param name="factFactory"> A reference to the factor creating these facts. </param>
        private void InitialiseBasicPackage(FactFactory factFactory)
        {
            factFactory.AddFact("nothing", () => false);  // Always has a belief, even if it never will successfully evaluate.
            factFactory.AddFact("is_outside", () => ActorManager.Instance.TryGetNode(Actor, out _));

            factFactory.AddFact("is_fresh", () => Actor.StaminaStat.Percent >= 0.9f);
            factFactory.AddFact("is_tired", () => Actor.StaminaStat.Percent < 0.5f);
            factFactory.AddFact("is_entertained", () => Actor.EntertainmentStat.Percent >= 0.9f);
            factFactory.AddFact("is_bored", () => Actor.EntertainmentStat.Percent < 0.5f);

            AvailableActions.Add(new ActorAction.Builder("Relax", new IdleStrategy(Actor, 5f))
                .AddOutcome(AvailableFacts["nothing"])
                .Build());

            AvailableActions.Add(new ActorAction.Builder("Wander", new WanderStrategy(Actor))
                .WithCost(() => 10f)    // TODO - Have calculated from actor personality.
                .AddOutcome(AvailableFacts["is_entertained"])
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
                factFactory.AddPositionFact($"at_{location.FormattedName}", 1f, location);
                factFactory.AddFact($"in_{location.FormattedName}", () => Actor.OccupyingLocation == location);

                // Add actions.
                AvailableActions.Add(new ActorAction.Builder($"goto_{location.FormattedName}", new GoToEntityStrategy(Actor, location))
                    .WithDistanceCost(Actor, location)
                    .AddPrecondition(AvailableFacts[$"is_outside"])
                    .AddOutcome(AvailableFacts[$"at_{location.FormattedName}"])
                    .Build());

                AvailableActions.Add(new ActorAction.Builder($"enter_{location.FormattedName}", new EnterLocationStrategy(Actor, location))
                    .WithCost(1f)
                    .AddPrecondition(AvailableFacts[$"at_{location.FormattedName}"])
                    .AddOutcome(AvailableFacts[$"in_{location.FormattedName}"])
                    .Build());

                AvailableActions.Add(new ActorAction.Builder($"exit_{location.FormattedName}", new ExitLocationStrategy(Actor, location))
                    .WithCost(1f)
                    .AddPrecondition(AvailableFacts[$"in_{location.FormattedName}"])
                    .AddOutcome(AvailableFacts[$"is_outside"])
                    .Build());  // TODO - Circular means that the planner can't build a complete path to evaluate, it cycles infinitely.

            }
        }


        /// <summary> Initialise all the facts and actions based upon actors within the game world. </summary>
        /// <param name="factFactory"> A reference to the factor creating these facts. </param>
        /// <param name="actors"> All the actors within the game world. </param>
        private void InitialiseActorPackage(FactFactory factFactory, ActorData[] actors)
        {
            foreach (ActorData actor in actors)
            {
                if(actor != Actor)  // Don't add facts about yourself!
                {
                    // Add facts.
                    factFactory.AddAwarenessFact($"sees_{actor.FormattedName}", actor);
                    factFactory.AddPositionFact($"at_{actor.FormattedName}", 1f, actor);

                    // Add actions.
                    AvailableActions.Add(new ActorAction.Builder($"find_{actor.FormattedName}", new FindEntityStrategy(Actor, actor))
                        // TODO - Add cost.
                        .AddOutcome(AvailableFacts[$"sees_{actor.FormattedName}"])
                        .Build());

                    AvailableActions.Add(new ActorAction.Builder($"goto_{actor.FormattedName}", new GoToEntityStrategy(Actor, actor))
                        .WithDistanceCost(Actor, actor)
                        .AddPrecondition(AvailableFacts[$"sees_{actor.FormattedName}"])
                        .AddOutcome(AvailableFacts[$"at_{actor.FormattedName}"])
                        .Build());
                }
            }
        }


        /// <summary> Set's the agent's initial goals relating to basic upkeep. </summary>
        private void InitialiseBasicGoals()
        {
            AvailableGoals = new HashSet<ActorGoal>();

            AvailableGoals.Add(new ActorGoal.Builder("WatchPaintDry", GoalSource.BASIC)
                .WithPriority(GoalPriority.NONE)
                .WithDesiredOutcome(AvailableFacts["nothing"])
                .Build());

            AvailableGoals.Add(new ActorGoal.Builder("KeepEntertained", GoalSource.BASIC)
                .WithPriority(GoalPriority.CRITICAL)
                .WithDesiredOutcome(AvailableFacts["is_entertained"])
                .Build());
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
                GD.Print($"{Actor.Name} -> Calculating new plan...");
                CalculatePlan();

                if (CurrentPlan != null && CurrentPlan.Actions.Count > 0)
                {
                    CurrentGoal = CurrentPlan.ActorGoal;
                    GD.Print($"{Actor.Name} -> Goal: {CurrentGoal.Name} with {CurrentPlan.Actions.Count} actions in plan.");

                    CurrentAction = CurrentPlan.Actions.Pop();
                    GD.Print($"{Actor.Name} -> Popped action: {CurrentAction.Name}.");

                    // Verify all precondition effects are true
                    if (CurrentAction.Preconditions.All(b => b.Evaluate()))
                    {
                        CurrentAction.Start();
                    }
                    else
                    {
                        GD.Print($"{Actor.Name} -> Goal preconditions not met, clearing current action and goal.");

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
                    GD.Print($"{Actor.Name} -> Action, {CurrentAction.Name}, complete.");

                    CurrentAction.Stop();
                    CurrentAction = null;

                    if (CurrentPlan.Actions.Count == 0)
                    {
                        GD.Print($"{Actor.Name} -> Plan complete!");

                        ArchiveCurrentGoal();
                    }
                }
            }
        }


        /// <summary> Attempt to calculate a new plan. </summary>
        private void CalculatePlan()
        {
            GoalPriority priorityLevel = CurrentGoal != null ? CurrentGoal.Priority : GoalPriority.NONE;

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
