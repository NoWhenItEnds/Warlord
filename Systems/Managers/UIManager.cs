using Godot;
using Warlord.Entities.Nodes.Locations;
using Warlord.UI.Windows;
using Warlord.Utilities.Singletons;

namespace Warlord.Managers
{
    /// <summary> The manager for the game world's UI. </summary>
    public partial class UIManager : SingletonControl<UIManager>
    {
        /// <summary> The popup node for a selected location. </summary>
        [ExportGroup("Nodes")]
        [Export] private LocationSelection _locationSelection;

        /// <summary> The window used for displaying actor cards. </summary>
        [Export] private ActorCardWindow _actorCardWindow;

        /// <summary> The window for displaying the state of the game world. </summary>
        [Export] private StatusWindow _statusWindow;


        /// <summary> Toggle the UI's location selector's target. </summary>
        /// <param name="location"> The location to follow. A null indicates to turn the selector off. </param>
        public void ToggleLocationSelection(LocationNode? location) => _locationSelection.Toggle(location);


        /// <summary> Get the dimensions of the screen that is interactable / usable. This is the area of the screen not currently blocked by long standing UI. </summary>
        /// <returns> The dimensions of the free screen. </returns>
        public Rect2 GetUsableScreen()
        {
            Rect2 totalRect = GetViewportRect();

            // Remove actor card window.
            if(_actorCardWindow.Visible)
            {
                totalRect.Size -= new Vector2(0, _actorCardWindow.Size.Y);
            }

            // Remove status window.
            if (_statusWindow.Visible)
            {
                totalRect.Size -= new Vector2(_statusWindow.Size.X, 0);
            }
            return totalRect;
        }
    }
}
