using Godot;
using System;

namespace Warlord.Nodes
{
    /// <summary> A graph visualising information as a radar polyline. </summary>
    [Tool]
    [GlobalClass]
    public partial class PolylineRadarChart : RadarChart
    {
        /// <summary> The width of the drawn line. </summary>
        protected Int32 _lineWidth = -1;

        /// <summary> The width of the drawn line. </summary>
        [Export] public Int32 LineWidth { get { return _lineWidth; } set { _lineWidth = value; QueueRedraw(); } }

        /// <summary> Whether the drawn line should be antialiased or not. </summary>
        protected Boolean _isAntialiased = true;

        /// <summary> Whether the drawn line should be antialiased or not. </summary>
        [Export] public Boolean IsAntialiased { get { return _isAntialiased; } set { _isAntialiased = value; QueueRedraw(); } }

        /// <summary> Whether the drawn line should have points drawn at the corners. </summary>
        protected Boolean _drawPoints = false;

        /// <summary> Whether the drawn line should have points drawn at the corners. </summary>
        [Export] public Boolean DrawPoints { get { return _drawPoints; } set { _drawPoints = value; QueueRedraw(); } }


        /// <inheritdoc/>
        public override void _Draw()
        {
            Vector2[] points = CalculatePoints();
            if (points.Length > 0)
            {
                DrawPolyline(points, _colour, _lineWidth, _isAntialiased);
                if (_drawPoints)
                {
                    foreach (Vector2 point in points)
                    {
                        DrawCircle(point, _lineWidth, _colour, antialiased: _isAntialiased);
                    }
                }
            }
        }
    }
}
