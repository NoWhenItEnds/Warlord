using Godot;
using System;
using System.Collections.Generic;
using Warlord.City.Generation.Fields;
using Warlord.Nodes;

namespace Warlord.City.Generation
{
    /// <summary> Generates cities for use within the game world using tensors. </summary>
    public partial class CityGenerator : Node
    {
        /// <summary> The line renderer to draw the grid lines. </summary>
        [ExportGroup("Nodes")]
        [Export] private LineDrawer3D _lines;

        /// <summary> The point renderer to draw the grid points. </summary>
        [Export] private PointDrawer3D _points;

        /// <summary> The point renderer to draw the tensor points. </summary>
        [Export] private PointDrawer3D _tensorPoints;


        /// <summary> The size of the city grid in Godot meters. </summary>
        [ExportGroup("Settings")]
        [Export] private Vector2 _cityDimension = new Vector2(100f, 100f);

        /// <summary> The world position of the center of the grid. </summary>
        [Export] private Vector2 _cityOrigin = Vector2.Zero;

        /// <summary> The size of each grid cell in Godot meters. </summary>
        [Export] private Single _gridDiameter = 1f;

        /// <summary> The gradient to use for rendering the tensor grid at various rotations. </summary>
        [Export] private Gradient _gridGradient;


        /// <summary> An internal reference to the tensor field the generator is currently using. </summary>
        private TensorField _tensorField;

        private Roads _highways;

        private Roads _mainRoads;

        private Roads _minorRoads;


        /// <inheritdoc/>
        public override void _Ready()
        {
            Vector2 actualOrigin = _cityOrigin - (_cityDimension * 0.5f);
            _tensorField = new TensorField(_cityDimension, actualOrigin, _gridDiameter);

            var integrator = new Integrator(_tensorField, 1f);
            _highways = new Roads(StreamLinesParams.Default, integrator, _cityDimension, actualOrigin);
            _mainRoads = new Roads(StreamLinesParams.Default.SetSeparation(5f).SetLookahead(500f), integrator,
            _cityDimension,
            actualOrigin);
            _minorRoads = new Roads(StreamLinesParams.Default.SetSeparation(2.5f).SetLookahead(500f), integrator,
                _cityDimension,
                actualOrigin);

            _minorRoads.SetExistingStreamlines(new List<Roads> { _highways, _mainRoads });
            _mainRoads.SetExistingStreamlines(new List<Roads> { _highways });

            Render();
        }


        /// <summary> Draw the various tensors used for city generation. </summary>
        private void Render()
        {
            _lines.Clear();
            _points.Clear();
            _tensorPoints.Clear();

            foreach (Vector2 point in _tensorField.Points)
            {
                Tensor tensor = _tensorField.GetPoint(point);
                Color colour = _gridGradient.Sample((tensor.Major.Angle() + Mathf.Pi) / (2 * Mathf.Pi));    // Make angle a percentage of radium (2 * Pi).
                _lines.AddLines(_tensorField.GetTensorLine(point, tensor.Major), colour);
                _lines.AddLines(_tensorField.GetTensorLine(point, tensor.Minor), colour);
            }

            foreach (Field field in _tensorField.Fields)
            {
                _tensorPoints.AddPoint(new Vector3(field.Center.X, 0, field.Center.Y), Colors.GreenYellow);
            }

            _highways.DrawLineRoads(_lines, Colors.Blue);
            _mainRoads.DrawLineRoads(_lines, Colors.Green);
            _minorRoads.DrawLineRoads(_lines, Colors.Red);
        }


        public void AddPoint(Vector2 position)
        {
            _tensorField.AddGrid(position, 20, 45, 2);
            Render();
        }

        public void RemovePoint(Vector2 position)
        {
            _tensorField.RemoveField(position);
            Render();
        }


        public void GenerateHighways()
        {
            _highways.GenerateRoads();
            Render();
        }

        public void GenerateMainRoads()
        {
            _mainRoads.GenerateRoads();
            Render();
        }

        public void GenerateMinorRoads()
        {
            _minorRoads.GenerateRoads();
            Render();
        }
    }
}
