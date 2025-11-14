using Godot;
using System;
using System.Collections.Generic;

namespace Warlord.Nodes
{
    [GlobalClass]
    public partial class LineDrawer3D : MeshInstance3D
    {
        private readonly List<(Vector3 Position, Color Colour)> _points = new List<(Vector3 Position, Color Colour)>();

        private ImmediateMesh _mesh = new ImmediateMesh();


        public void AddLine(Vector3 p1, Vector3 p2, Color colour)
        {
            _points.Add(new (p1, colour));
            _points.Add(new (p2, colour));
        }


        public void AddLine(Vector3 p1, Vector3 p2)
        {
            AddLine(p1, p2, Colors.Red);
        }


        public void AddLines(Vector2[] line, Color colour)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(line.Length, 1);

            foreach (Vector2 point in line)
            {
                _points.Add(new (new Vector3(point.X, point.Y, 0), colour));
            }
        }


        public void AddLines(Vector2[] line)
        {
            AddLines(line, Colors.Red);
        }


        public void Clear()
        {
            _mesh.ClearSurfaces();
            _points.Clear();
        }


        /// <inheritdoc/>
        public override void _Ready()
        {
            StandardMaterial3D material = new StandardMaterial3D();
            material.NoDepthTest = true;
            material.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
            material.VertexColorUseAsAlbedo = true;
            material.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
            MaterialOverride = material;
        }


        /// <inheritdoc/>
        public override void _Process(Double delta)
        {
            // Redraw the lines.
            if(_points.Count > 0)
            {
                _mesh.ClearSurfaces();
                _mesh.SurfaceBegin(Mesh.PrimitiveType.Lines);

                foreach ((Vector3 Position, Color Colour) point in _points)
                {
                    _mesh.SurfaceSetColor(point.Colour);
                    _mesh.SurfaceAddVertex(point.Position);
                }

                _mesh.SurfaceEnd();
                Mesh = _mesh;
            }
        }
    }
}
