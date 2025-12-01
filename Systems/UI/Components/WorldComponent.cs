using Godot;
using System;
using Warlord.Managers;

namespace Warlord.UI.Components
{
    /// <summary> Displays information about the game world. </summary>
    public partial class WorldComponent : Control
    {
        /// <summary> The button to pause the game. </summary>
        [ExportGroup("Nodes")]
        [ExportSubgroup("Time Controls")]
        [Export] private Button _pauseButton;

        /// <summary> The button to set the game to real time. </summary>
        [Export] private Button _realTimeButton;

        /// <summary> The button to set the game to normal time. </summary>
        [Export] private Button _normalTimeButton;

        /// <summary> The button to set the game to fast time. </summary>
        [Export] private Button _fastTimeButton;

        /// <summary> The button to set the game to skip time. </summary>
        [Export] private Button _skipTimeButton;


        /// <summary> Label used to display the current game time. </summary>
        [ExportSubgroup("Labels")]
        [Export] private RichTextLabel _timeLabel;

        /// <summary> Label used to display the current game date. </summary>
        [Export] private RichTextLabel _dateLabel;


        /// <summary> A reference to the game time. </summary>
        private TimeManager _timeManager;


        /// <summary> The format to use to display the time. </summary>
        private const String TIME_FORMAT = "{0:HH:mm}";

        /// <summary> The format to use to display the date. </summary>
        private const String DATE_FORMAT = "{0:dddd}, {0:dd MMMM}";


        /// <inheritdoc/>
        public override void _Ready()
        {
            _timeManager = TimeManager.Instance;

            _pauseButton.ButtonDown += OnPause;
            _realTimeButton.ButtonDown += OnRealTime;
            _normalTimeButton.ButtonDown += OnNormalTime;
            _fastTimeButton.ButtonDown += OnFastTime;
            _skipTimeButton.ButtonDown += OnSkipTime;
        }


        /// <inheritdoc/>
        public override void _Process(Double delta)
        {
            _timeLabel.Text = String.Format(TIME_FORMAT, _timeManager.CurrentTime);
            _dateLabel.Text = String.Format(DATE_FORMAT, _timeManager.CurrentTime);
        }


        private void OnPause() => _timeManager.SetTimescale(0f);
        private void OnRealTime() => _timeManager.SetTimescale(1f);
        private void OnNormalTime() => _timeManager.SetTimescale(60f);
        private void OnFastTime() => _timeManager.SetTimescale(120f);
        private void OnSkipTime() => _timeManager.SetTimescale(600f);


        /// <inheritdoc/>
        public override void _ExitTree()
        {
            _pauseButton.ButtonDown -= OnPause;
            _realTimeButton.ButtonDown -= OnRealTime;
            _normalTimeButton.ButtonDown -= OnNormalTime;
            _fastTimeButton.ButtonDown -= OnFastTime;
            _skipTimeButton.ButtonDown -= OnSkipTime;
        }
    }
}
