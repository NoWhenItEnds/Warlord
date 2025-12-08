using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Warlord.Entities.GOAP;
using Warlord.Entities.Nodes;
using Warlord.Entities.Resources;
using Warlord.Managers;
using Warlord.Organisations.Objectives;

namespace Warlord.Organisations
{
    /// <summary> An organisation within the game world that manipulates actors. </summary>
    [GlobalClass]
    public partial class OrganisationController : Node
    {
        /// <summary> All the actors that is organisation has control of. </summary>
        private HashSet<ActorData> _controlledActors = new HashSet<ActorData>();

        /// <summary> An array of the organisation's objectives. </summary>
        private HashSet<OrganisationObjective> _objectives = new HashSet<OrganisationObjective>();

        /// <summary> When the organisation's objectives are changed or reordered. </summary>
        public Action<OrganisationObjective[]> ObjectivesUpdated;


        /// <summary> A reference to the game's actor manager. </summary>
        private ActorManager _actorManager;


        /// <inheritdoc/>
        public override void _Ready()
        {
            _actorManager = ActorManager.Instance;

            // TODO - NOT LIKE THIS. HAVE SELECTOR. Have spawnner.
            if(_actorManager.TryGetData("actor_skitter", out ActorData? skitter))
            {
                AddActor(skitter);
            }
            if (_actorManager.TryGetData("actor_tattletale", out ActorData? tattletale))
            {
                AddActor(tattletale);
            }

            ActorNode skitterNode = _actorManager.SpawnNode(skitter, new Vector3(1, 1, 1));
            ActorNode tattletaleNode = _actorManager.SpawnNode(tattletale, new Vector3(-1, 1, -1));
        }


        public void AddActor(ActorData actor)
        {
            if(_controlledActors.Add(actor))
            {
                ActorController controller = _actorManager.GetController(actor);
                foreach (OrganisationObjective objective in _objectives)
                {
                    objective.AddGoal(controller);
                }
            }
        }


        public void RemoveActor(ActorData actor)
        {
            if (_controlledActors.Remove(actor))
            {
                ActorController controller = _actorManager.GetController(actor);
                foreach (OrganisationObjective objective in _objectives)
                {
                    objective.TryRemoveGoal(controller);
                }
            }
        }



        /// <summary> Get all the actors controlled by this organisation. </summary>
        /// <returns> An array of the actors controlled by this organisation. </returns>
        public ActorData[] GetActors() => _controlledActors.ToArray();


        public void AddObjective(OrganisationObjective objective)
        {
            if(_objectives.Add(objective))
            {
                foreach (ActorData actor in _controlledActors)
                {
                    ActorController controller = _actorManager.GetController(actor);
                    objective.AddGoal(controller);
                }

                ObjectivesUpdated?.Invoke(_objectives.ToArray());
            }
        }


        public void RemoveObjective(OrganisationObjective objective)
        {
            if(_objectives.Remove(objective))
            {
                foreach (ActorData actor in _controlledActors)
                {
                    ActorController controller = _actorManager.GetController(actor);
                    objective.TryRemoveGoal(controller);
                }

                ObjectivesUpdated?.Invoke(_objectives.ToArray());
            }
        }


        public OrganisationObjective[] GetObjectives() => _objectives.ToArray();
    }
}
