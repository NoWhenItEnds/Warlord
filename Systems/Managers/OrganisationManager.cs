using System.Collections.Generic;
using Godot;
using Warlord.Organisations;
using Warlord.Utilities.Singletons;

namespace Warlord.Managers
{
    /// <summary> Managers the various organisations within the game world. </summary>
    public partial class OrganisationManager : SingletonNode<OrganisationManager>
    {
        /// <summary> The controller used by player 'one'. All other organisations, from their perspective, could be AI. </summary>
        [ExportGroup("Nodes")]
        [Export] public OrganisationController PlayerController { get; private set; }

        /// <summary> The controllers for the other organisations within the game world. </summary>
        private List<OrganisationController> _otherControllers = new List<OrganisationController>();
    }
}
