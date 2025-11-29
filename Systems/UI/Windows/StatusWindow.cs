using Godot;
using System;
using Warlord.UI.Components;

namespace Warlord.UI.Windows
{
    /// <summary> A status bar showing information about the state of the game world. </summary>
    public partial class StatusWindow : Control
    {
        /// <summary> Displays information about the game world. </summary>
        [ExportGroup("Nodes")]
        [Export] private WorldComponent _worldComponent;
    }
}
