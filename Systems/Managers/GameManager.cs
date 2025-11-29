using System;
using Godot;
using Warlord.Utilities.Extensions;
using Warlord.Utilities.Singletons;

namespace Warlord.Managers
{
    /// <summary> The manager singleton of the game scene. The program's entrypoint. </summary>
    public partial class GameManager : SingletonNode<GameManager>
    {
        /// <summary> A reference to the game world's environment. </summary>
        [ExportGroup("Nodes")]
        [Export] private WorldEnvironment _environment;

        /// <summary> A reference to the directional light representing sunlight. </summary>
        [Export] private DirectionalLight3D _sunlight;


        /// <summary> How quickly the game time advances relative to real time. </summary>
        [ExportGroup("Settings")]
        [Export] private Single _timescale = 60f;

        /// <summary> The longitude and latitude, in that order, of the world's location. </summary>
        [Export] private Vector2 _geographicalLocation = Vector2.Zero;

        /// <summary> The gradient of the sun's colour from midnight to midday. </summary>
        [Export] private Gradient _sunlightColour;


        /// <summary> The game's current time. </summary>
        public DateTimeOffset CurrentTime { get; private set; } = new DateTimeOffset(2000, 1, 1, 6, 0, 0, TimeSpan.Zero);


        /// <inheritdoc/>
        public override void _Ready()
        {
            GD.Print("Hello, World!");
        }


        /// <inheritdoc/>
        public override void _Process(Double delta)
        {
            CurrentTime = CurrentTime.AddSeconds(delta * _timescale);
            _sunlight.RotationDegrees = CurrentTime.CalculateSunRotation(_geographicalLocation.Y, _geographicalLocation.X); // TODO - Fix rotation resetting at sunset.
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
    }
}
