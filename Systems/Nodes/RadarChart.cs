using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

namespace Warlord.Nodes
{
    /// <summary> A graph visualising information as a radar polygon. </summary>
    [Tool]
    [GlobalClass]
    public partial class RadarChart : Control
    {
        /// <summary> The data to display. Note, it should be relative (0.0 - 1.0). There also needs to be at least 3 elements. </summary>
        protected Array<Single> _data = new Array<Single>();

        /// <summary> The data to display. Note, it should be relative (0.0 - 1.0). There also needs to be at least 3 elements. </summary>
        [Export] public Array<Single> Data { get { return _data; } set { _data = value; QueueRedraw(); } }

        /// <summary> The colour of the chart. </summary>
        protected Color _colour = Colors.White;

        /// <summary> The colour of the chart. </summary>
        [Export] public Color Colour { get { return _colour; } set { _colour = value; QueueRedraw(); } }

        /// <summary> Constant to use for rotating. </summary>
        private const Double ROTATION_OFFSET = -Math.PI * 0.5;


        /// <inheritdoc/>
        public override void _Draw()
        {
            Vector2[] points = CalculatePoints();
            if (points.Length > 0)
            {
                DrawPolygon(points, [_colour]);
            }
        }


        /// <summary> Calculate the graph's points to build the chart. </summary>
        /// <returns> An array of positions for each point on the chart. </returns>
        protected Vector2[] CalculatePoints()
        {
            List<Vector2> points = new List<Vector2>();
            Int32 sides = _data.Count;
            if (sides < 3)
            {
                GD.PushWarning("Unable to calculate radar chart. It must have at least 3 points of data.");
                return new Vector2[0];
            }

            Double angleStep = Math.Tau / sides;
            Vector2 positionOffset = Size * 0.5f;

            for (Int32 i = 0; i < sides; i++)
            {
                Double angle = i * angleStep + ROTATION_OFFSET;
                Vector2 point = new Vector2((Single)Math.Cos(angle), (Single)Math.Sin(angle)) * Size.Y * 0.5f * _data[i] + positionOffset;
                points.Add(point);
            }

            points.Add(points[0]);    // Close the polygon.
            return points.ToArray();
        }
    }
}
