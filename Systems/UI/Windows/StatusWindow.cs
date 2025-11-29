using Godot;
using Warlord.Entities.Data;
using Warlord.UI.Components;

namespace Warlord.UI.Windows
{
    /// <summary> A status bar showing information about the state of the game world. </summary>
    public partial class StatusWindow : Control
    {
        /// <summary> Displays information about the game world. </summary>
        [ExportGroup("Nodes")]
        [Export] private WorldComponent _worldComponent;

        /// <summary> Displays information about the currently selected actor. </summary>
        [Export] private ActorComponent _actorComponent;


        /// <summary> Set the currently selected actor. </summary>
        /// <param name="actor"> The selected actor. A null indicates that there isn't one. </param>
        public void ToggleActorSelection(ActorData? actor)
        {
            _actorComponent.Toggle(actor);
            // TODO - If the actor is null, another component showing general information should be shown instead.
        }
    }
}
