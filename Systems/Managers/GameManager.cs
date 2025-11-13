using Godot;
using Warlord.Utilities.Singletons;

namespace Warlord.Managers
{
    /// <summary> The manager singleton of the game scene. The program's entrypoint. </summary>
    public partial class GameManager : SingletonNode<GameManager>
    {
        /// <inheritdoc/>
        public override void _Ready()
        {
            GD.Print("Hello, World!");
        }
    }
}
