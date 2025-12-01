using System;
using Godot;
using Warlord.Utilities.Singletons;

namespace Warlord.Managers
{
    /// <summary> The manager singleton of the game scene. The program's entrypoint. </summary>
    public partial class GameManager : SingletonNode<GameManager>
    {
        /// <summary> The geographical position of the game world on the North-South axis. </summary>
        [ExportGroup("Settings")]
        [Export] public Single Latitude { get; private set; }

        /// <summary> The geographical position of the game world on the East-West axis. </summary>
        [Export] public Single Longitude { get; private set; }


        /// <inheritdoc/>
        public override void _Ready()
        {
            GD.Print("Hello, World!");
        }
    }
}
