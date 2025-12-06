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

        /// <summary> An array of the queued objectives sorted priorities. </summary>
        private List<OrganisationObjective> _queuedObjectives = new List<OrganisationObjective>();

        /// <summary> When the organisation's objectives are changed or reordered. </summary>
        public Action<OrganisationObjective[]> ObjectivesUpdated;


        /// <summary> A reference to the game's actor manager. </summary>
        private ActorManager _actorManager;


        /// <inheritdoc/>
        public override void _Ready()
        {
            _actorManager = ActorManager.Instance;

            // TODO - NOT LIKE THIS. HAVE SELECTOR. Have spawnner.
            if(_actorManager.TryGetData("Skitter", out ActorData? skitter))
            {
                AddActor(skitter);
            }
            if (_actorManager.TryGetData("Tattletale", out ActorData? tattletale))
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
            }
        }


        public void RemoveActor(ActorData actor)
        {
            if (_controlledActors.Remove(actor))
            {
            }
        }



        /// <summary> Get all the actors controlled by this organisation. </summary>
        /// <returns> An array of the actors controlled by this organisation. </returns>
        public ActorData[] GetActors() => _controlledActors.ToArray();


        public void AddObjective(OrganisationObjective objective, Int32 index = -1)
        {
            if(index < 0 || index > _queuedObjectives.Count - 1)
            {
                _queuedObjectives.Add(objective);
            }
            else
            {
                _queuedObjectives.Insert(index, objective);
            }

            _queuedObjectives.Sort();   // TODO - How to sort.
            ObjectivesUpdated?.Invoke(_queuedObjectives.ToArray());
            UpdateActorObjectives();
        }


        public void RemoveObjective(OrganisationObjective objective)
        {
            if(_queuedObjectives.Remove(objective))
            {
                _queuedObjectives.Sort();   // TODO - How to sort.
                ObjectivesUpdated?.Invoke(_queuedObjectives.ToArray());
                UpdateActorObjectives();
            }
        }

        private void UpdateActorObjectives()
        {
            if(_queuedObjectives.Count > 0)
            {
                OrganisationObjective current = _queuedObjectives.First();
                foreach (ActorData actor in _controlledActors)
                {
                    ActorController controller = _actorManager.GetController(actor);
                    current.AddGoal(controller);
                }
            }
        }


        public OrganisationObjective[] GetObjectives() => _queuedObjectives.ToArray();
    }
}
