using Godot;
using System;
using Warlord.City.Generation.Fields;
using Warlord.Nodes;

namespace Warlord.City.Generation
{
    /// <summary> Generates cities for use within the game world. </summary>
    public partial class CityGenerator : Node3D
    {
        [ExportGroup("Nodes")]
        [Export] private LineDrawer3D _lines;

        [Export] private PointDrawer3D _points;

        [Export] private PointDrawer3D _tensorPoints;


        /// <summary> The size of the city grid in godot units. </summary>
        [ExportGroup("Settings")]
        [Export] private Vector2 _cityDimension = new Vector2(100f, 100f);

        /// <summary> The position of the bottom-left corner of the tensor grid. </summary>
        [Export] private Vector2 _cityOrigin = Vector2.Zero;


        private TensorField _tensorField;


        /// <inheritdoc/>
        public override void _Ready()
        {
            _tensorField = new TensorField(_cityDimension * 0.5f, _cityOrigin);

            Redraw();
        }


        private void Redraw()
        {
            _lines.Clear();
            _points.Clear();
            _tensorPoints.Clear();

            foreach (Vector2 point in _tensorField.Points)
            {
                Tensor tensor = _tensorField.GetPoint(point);
                _lines.AddLines(_tensorField.GetTensorLine(point, tensor.Major));
                _lines.AddLines(_tensorField.GetTensorLine(point, tensor.Minor));
            }

/*
            foreach (Field field in _tensorField.Fields)
                if (selectedField != null && selectedField.Equals(field))
                    _tensorPoints.AddPoint(new Vector3(field.center.X, field.center.Y, 0), Colors.GreenYellow);
                else
                    _tensorPoints.AddPoint(new Vector3(field.center.X, field.center.Y, 0), Colors.Yellow);*/
        }
    }
}
