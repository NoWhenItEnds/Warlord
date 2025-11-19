using Godot;
using System;
using System.Collections.Generic;

namespace Warlord.Nodes
{
    /// <summary> A mesh node that renders points. </summary>
    [GlobalClass]
    public partial class PointDrawer3D : MeshInstance3D
    {
        /// <summary> The width to draw each point. </summary>
        [Export] private Single _pointWidth = 0.1f;


        /// <summary> An array that holds the points we will draw. </summary>
        private readonly List<(Vector3 Position, Color Colour)> _points = new List<(Vector3 Position, Color Colour)>();

        /// <summary> The mesh we will use to render the points. </summary>
        private ImmediateMesh _mesh = new ImmediateMesh();


        /// <summary> Adds a position to the renderer. </summary>
        /// <param name="point"> A position to draw a point at. </param>
        public void AddPoint(Vector3 point)
        {
            AddPoint(point, Colors.Green);
        }


        /// <summary> Adds a position to the renderer. </summary>
        /// <param name="point"> A position to draw a point at. </param>
        /// <param name="colour"> The colour to colour the point. </param>
        public void AddPoint(Vector3 point, Color colour)
        {
            _points.Add(new (point, colour));
        }


        /// <summary> Clear the drawer. </summary>
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
            if(_points.Count > 0)
            {
                _mesh.ClearSurfaces();
                _mesh.SurfaceBegin(Mesh.PrimitiveType.Triangles);

                foreach ((Vector3 Position, Color Colour) point in _points)
                {
                    _mesh.SurfaceSetColor(point.Colour);
                    _mesh.SurfaceAddVertex(point.Position + new Vector3(-1 * _pointWidth, 0f, 1 * _pointWidth));    // Bottom Left.
                    _mesh.SurfaceAddVertex(point.Position + new Vector3(-1 * _pointWidth, 0f, -1 * _pointWidth));   // Top Left.
                    _mesh.SurfaceAddVertex(point.Position + new Vector3(1 * _pointWidth, 0f, -1 * _pointWidth));    // Top Right.

                    _mesh.SurfaceAddVertex(point.Position + new Vector3(-1 * _pointWidth, 0f, 1 * _pointWidth));    // Bottom Left.
                    _mesh.SurfaceAddVertex(point.Position + new Vector3(1 * _pointWidth, 0f, -1 * _pointWidth));    // Top Right
                    _mesh.SurfaceAddVertex(point.Position + new Vector3(1 * _pointWidth, 0f, 1 * _pointWidth));     // Bottom Right.
                }

                _mesh.SurfaceEnd();

                Mesh = _mesh;
            }
        }
    }
}
