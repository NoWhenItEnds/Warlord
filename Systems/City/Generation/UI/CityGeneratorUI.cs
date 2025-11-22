using Godot;
using System;

namespace Warlord.City.Generation.UI
{
    /// <summary> The main UI for the city generator. </summary>
    public partial class CityGeneratorUI : Control
    {
        /// <summary> A reference to the main city generator. </summary>
        [ExportGroup("Nodes")]
        [Export] private CityGenerator _generator;

        [Export] private Button _regenerateGrid;

        [Export] private Button _generateHighways;

        [Export] private Button _generateMainRoads;

        [Export] private Button _generateMinorRoads;

        public override void _Ready()
        {
            _generateHighways.ButtonDown += _generator.GenerateHighways;
            _generateMainRoads.ButtonDown += _generator.GenerateMainRoads;
            _generateMinorRoads.ButtonDown += _generator.GenerateMinorRoads;
        }


        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton button && button.IsPressed())
            {
                Camera3D camera = GetViewport().GetCamera3D();  // TODO - Make standalone scene camera.
                Vector3 position = camera.ProjectPosition(button.Position, Mathf.Abs(camera.GlobalPosition.Y));

                if(button.ButtonIndex == MouseButton.Left)
                {
                    _generator.AddPoint(new Vector2(position.X, position.Z));
                }
                else if (button.ButtonIndex == MouseButton.Right)
                {
                    _generator.RemovePoint(new Vector2(position.X, position.Z));
                }
            }
        }


        public override void _ExitTree()
        {
            _generateHighways.ButtonDown -= _generator.GenerateHighways;
            _generateMainRoads.ButtonDown -= _generator.GenerateMainRoads;
            _generateMinorRoads.ButtonDown -= _generator.GenerateMinorRoads;
        }

    }
}
