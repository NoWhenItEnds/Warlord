using System;
using System.Collections.Generic;
using System.Linq;
using Warlord.Entities.GOAP.Strategies;
using Warlord.Entities.Resources;

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
        public Dictionary<String, ActorFact> AvailableBeliefs { get; private set; }

        /// <summary> The goals that the actor will seek to address. </summary>
        public HashSet<ActorGoal> AvailableGoals { get; private set; }

        /// <summary> The potential actions this actor has access to. </summary>
        public HashSet<ActorAction> AvailableActions { get; private set; }


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

            InitialiseBeliefs();
            InitialiseActions();
            InitialiseGoals();
        }


        /// <summary> Set's the actor's initial beliefs. </summary>
        private void InitialiseBeliefs()    // TODO - These should be pulled from a JSON or something.
        {
            AvailableBeliefs = new Dictionary<String, ActorFact>();   // Initialise a new set of beliefs by clearing the current.
            FactFactory factory = new FactFactory(ACTOR, AvailableBeliefs);

            factory.AddFact("Nothing", () => false);  // Always has a belief, even if it never will successfully evaluate.

            //factory.AddFact("IsIdle", () => ACTOR.NavigationAgent.IsNavigationFinished());
            //factory.AddFact("IsMoving", () => !ACTOR.NavigationAgent.IsNavigationFinished());

            //factory.AddFact("IsHealthy", () => ACTOR.CurrentHealth >= 90f);
            //factory.AddFact("IsHurt", () => ACTOR.CurrentHealth < 50);
            //factory.AddFact("IsFed", () => ACTOR.CurrentHunger >= 90f);
            //factory.AddFact("IsHungry", () => ACTOR.CurrentHunger < 50f);
            //factory.AddFact("IsRested", () => ACTOR.CurrentFatigue >= 90f);
            //factory.AddFact("IsTired", () => ACTOR.CurrentFatigue < 50f);
            //factory.AddFact("IsEntertained", () => ACTOR.CurrentEntertainment >= 90f);
            //factory.AddFact("IsBored", () => ACTOR.CurrentEntertainment < 50f);

            // TODO - Add belief packages. Such as food beliefs that contains both the Knows and Sees for the item.
            //factory.AddEntityFact<ItemNode>("KnowsApple", "item_food_apple");
            //factory.AddNodeLocationBelief<ItemNode>("AtApple", "item_food_apple", 1f); // TODO - This should be based upon something.
            //factory.AddInventoryBelief("HasApple", "item_food_apple");

            //factory.AddSensorBelief("AgentKnowsPlayer", _controlledEntity.Sensors, _playerController.PlayerUnit);
            //factory.AddBelief("AgentSeesPlayer", () => _controlledEntity.Sensors.TryGetEntity(_playerController.PlayerUnit).IsVisible);
        }


        /// <summary> Set's the actor's initial actions. </summary>
        private void InitialiseActions()
        {
            AvailableActions = new HashSet<ActorAction>();   // Initialise a new set of actions by clearing the current.

            AvailableActions.Add(new ActorAction.Builder("Relax", new IdleActionStrategy(ACTOR, 5f))
                .AddOutcome(AvailableBeliefs["Nothing"])
                .Build());

            /*
            AvailableActions.Add(new ActorAction.Builder("WanderAround")
                .WithStrategy(new WanderActionStrategy(ACTOR, 2f))
                .AddOutcome(AvailableBeliefs["IsMoving"])
                .Build());

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


        /// <summary> Set's the agent's initial goals. </summary>
        private void InitialiseGoals()
        {
            AvailableGoals = new HashSet<ActorGoal>();


            AvailableGoals.Add(new ActorGoal.Builder("ChillOut")
                .WithPriority(0)
                .WithDesiredOutcome(AvailableBeliefs["Nothing"])
                .Build());
/*
            AvailableGoals.Add(new ActorGoal.Builder("Wander")
                .WithPriority(0)
                .WithDesiredOutcome(AvailableBeliefs["IsMoving"])
                .Build());


            AvailableGoals.Add(new ActorGoal.Builder("KeepFed")
                .WithPriority(10)
                .WithDesiredOutcome(AvailableBeliefs["IsFed"])
                .Build());*/
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
                //_uiController.SpawnSpeechBubble("Calculating any potential new plan.", ACTOR);
                CalculatePlan();

                if (CurrentPlan != null && CurrentPlan.Actions.Count > 0)
                {
                    CurrentGoal = CurrentPlan.ActorGoal;
                    //_uiController.SpawnSpeechBubble($"Goal: {CurrentGoal.Name} with {CurrentPlan.Actions.Count} actions in plan", ACTOR);

                    CurrentAction = CurrentPlan.Actions.Pop();
                    //_uiController.SpawnSpeechBubble($"Popped action: {CurrentAction.Name}", ACTOR);

                    // Verify all precondition effects are true
                    if (CurrentAction.Preconditions.All(b => b.Evaluate()))
                    {
                        CurrentAction.Start();
                    }
                    else
                    {
                        //_uiController.SpawnSpeechBubble("Preconditions not met, clearing current action and goal", ACTOR);

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
                    //_uiController.SpawnSpeechBubble($"Action, {CurrentAction.Name}, complete.", ACTOR);

                    CurrentAction.Stop();
                    CurrentAction = null;

                    if (CurrentPlan.Actions.Count == 0)
                    {
                        //_uiController.SpawnSpeechBubble("Plan complete!", ACTOR);

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
