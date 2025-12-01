using Godot;
using System;
using Warlord.Utilities.Extensions;
using Warlord.Utilities.Singletons;

namespace Warlord.Managers
{
    /// <summary> Manages the game world's time. </summary>
    public partial class TimeManager : SingletonNode<TimeManager>
    {
        /// <summary> A reference to the game world's environment. </summary>
        [ExportGroup("Nodes")]
        [Export] private WorldEnvironment _environment;

        /// <summary> A reference to the directional light representing sunlight. </summary>
        [Export] private DirectionalLight3D _sunlight;

        /// <summary> How quickly the game time advances relative to real time. </summary>
        [ExportGroup("Settings")]
        [Export] private Single _timescale = 60f;

        /// <summary> The gradient of the sun's colour from midnight to midday. </summary>
        [Export] private Gradient _sunlightColour;


        /// <summary> The game's current time. </summary>
        public DateTimeOffset CurrentTime { get; private set; } = new DateTimeOffset(2000, 1, 1, 6, 0, 0, TimeSpan.Zero);

        /// <summary> The time that has passed since the previous 'tick'. </summary>
        public Double GameTimeDelta => (CurrentTime - _previousTime).TotalSeconds;

        /// <summary> The time in the game world the previous observed 'tick'. </summary>
        private DateTimeOffset _previousTime;

        /// <summary> A reference to the game manager. </summary>
        private GameManager _gameManager;


        /// <inheritdoc/>
        public override void _Ready()
        {
            _gameManager = GameManager.Instance;
            _previousTime = CurrentTime;
        }


        /// <inheritdoc/>
        public override void _Process(Double delta)
        {
            _previousTime = CurrentTime;
            CurrentTime = CurrentTime.AddSeconds(delta * _timescale);
            _sunlight.Basis = CurrentTime.GetSunDirectionRotation(_gameManager.Latitude, _gameManager.Longitude);
            UpdateSunlight();
        }


        /// <summary> Update the sunlight's colour and power. </summary>
        private void UpdateSunlight()
        {
            // Calculate sun elevation: -1 (below horizon) to +1 (zenith).
            Single elevation = -Mathf.Sin(_sunlight.Rotation.X);

            // Map elevation (-1 to +1) → gradient position (0 to 1).
            Single t = (elevation + 1f) * 0.5f; // 0 = deep night, 0.5 = horizon, 1 = noon.

            // Sample the gradients
            Color colour = _sunlightColour.Sample(t);
            Single energy = _sunlightColour.Sample(t).R; // Use red channel as scalar.

            _sunlight.LightColor = colour;
            _sunlight.LightEnergy = energy;
        }


        /// <summary> Set the world's game timescale. </summary>
        /// <param name="value"> The new timescale value. </param>
        public void SetTimescale(Single value) => _timescale = Mathf.Clamp(value, 0, 10000);
    }
}
