using Godot;
using System;
using Warlord.Managers;

namespace Warlord.UI.Components
{
    /// <summary> Displays information about the game world. </summary>
    public partial class WorldComponent : Control
    {
        /// <summary> Label used to display the current game time. </summary>
        [ExportGroup("Nodes")]
        [Export] private RichTextLabel _timeLabel;

        /// <summary> Label used to display the current game date. </summary>
        [Export] private RichTextLabel _dateLabel;


        /// <summary> A reference to the game manager. </summary>
        private GameManager _gameManager;


        /// <summary> The format to use to display the time. </summary>
        private const String TIME_FORMAT = "{0:HH:mm}";

        /// <summary> The format to use to display the date. </summary>
        private const String DATE_FORMAT = "{0:dddd}, {0:dd MMMM}";


        /// <inheritdoc/>
        public override void _Ready()
        {
            _gameManager = GameManager.Instance;
        }


        public override void _Process(Double delta)
        {
            _timeLabel.Text = String.Format(TIME_FORMAT, _gameManager.CurrentTime);
            _dateLabel.Text = String.Format(DATE_FORMAT, _gameManager.CurrentTime);
        }
    }
}
