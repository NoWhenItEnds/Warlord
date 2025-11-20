using Godot;
using System;
using System.Collections.Generic;

namespace Warlord.Utilities.Extensions
{
    /// <summary> Helpful methods and additional functionality for Vector2. </summary>
    public static class Vector2Extensions
    {
        public static Vector2[] Simplify(Vector2[] points, Single tolerance = 1f, bool highestQuality = false)
        {
            if (points.Length <= 2)
            {
                return points;
            }

            Single sqTolerance = tolerance * tolerance;
            points = highestQuality ? points : SimplifyRadialDistance(points, sqTolerance);
            points = SimplifyDouglasPeucker(points, sqTolerance);
            return points;
        }


        /// <summary> Simplify a line by throwing away any points too close to one another. </summary>
        /// <param name="points"> An array of points composing the line. </param>
        /// <param name="tolerance"> How far away a point must be from another before it is kept. </param>
        /// <returns> The simplified line. </returns>
        private static Vector2[] SimplifyRadialDistance(Vector2[] points, Single tolerance)
        {
            Vector2 point = Vector2.Zero;
            Vector2 point2 = points[0];

            List<Vector2> list = new List<Vector2> { point2 };
            for (Int32 i = 1; i < points.Length; i++)
            {
                point = points[i];
                if (point.DistanceSquaredTo(point2) > tolerance)
                {
                    list.Add(point);
                    point2 = point;
                }
            }

            if (point2 != point)
            {
                list.Add(point);
            }

            return list.ToArray();
        }


        /// <summary> Simplifies a polyline (or polygon) using the Ramer–Douglas–Peucker algorithm. </summary>
        /// <param name="points"> The input array of 2D points defining the polyline. Must contain at least 2 points. </param>
        /// <param name="tolerance"> Squared distance tolerance. Larger values produce more aggressive simplification. </param>
        /// <returns> A new array containing a subset of the original points that approximates the original shape within the given tolerance. The first and last points of the input are always preserved. </returns>
        private static Vector2[] SimplifyDouglasPeucker(Vector2[] points, Single tolerance)
        {
            Int32 length = points.Length - 1;
            List<Vector2> list = new List<Vector2> { points[0] };
            SimplifyDpStep(points, 0, length, tolerance, list);
            list.Add(points[length]);
            return list.ToArray();
        }


        /// <summary> A recursive helper method that implements teh core of the Douglas-Peucker algorithm for polyline simplification. </summary>
        /// <param name="points"> The original array of points representing the line. </param>
        /// <param name="first"> The starting index of the current segment. </param>
        /// <param name="last"> The final index of the current segment. </param>
        /// <param name="tolerance"> The distance allowed before a point is simplified. </param>
        /// <param name="simplified"> The output list being constructed. </param>
        private static void SimplifyDpStep(Vector2[] points, Int32 first, Int32 last, Single tolerance, List<Vector2> simplified)
        {
            Single num = tolerance;
            Int32 num2 = 0;
            for (Int32 i = first + 1; i < last; i++)
            {
                Single sqSegDist = GetSquaredSegmentDistance(points[i], points[first], points[last]);
                if (sqSegDist > num)
                {
                    num2 = i;
                    num = sqSegDist;
                }
            }

            if (num > tolerance)
            {
                if (num2 - first > 1)
                {
                    SimplifyDpStep(points, first, num2, tolerance, simplified);
                }

                simplified.Add(points[num2]);
                if (last - num2 > 1)
                {
                    SimplifyDpStep(points, num2, last, tolerance, simplified);
                }
            }
        }


        /// <summary> Get the squared euclidean distance of a line segment. </summary>
        /// <param name="pnt0">The point to measure the distance from.</param>
        /// <param name="pnt1">The first endpoint of the line segment.</param>
        /// <param name="pnt2">The second endpoint of the line segment.</param>
        /// <returns>
        /// The squared distance from <paramref name="pnt0"/> to the closest point on the segment defined by <paramref name="pnt1"/> and <paramref name="pnt2"/>.
        /// If the closest point lies on the infinite line outside the segment, the distance to the nearer endpoint is returned instead.
        /// </returns>
        private static Single GetSquaredSegmentDistance(Vector2 pnt0, Vector2 pnt1, Vector2 pnt2)
        {
            Single x1 = pnt1.X;
            Single y1 = pnt1.Y;
            Single dX = pnt2.X - x1;
            Single dY = pnt2.Y - y1;
            if (Math.Abs(dX) > 0.0 || Math.Abs(dY) > 0.0)
            {
                Single t = ((pnt0.X - x1) * dX + (pnt0.Y - y1) * dY) / (dX * dX + dY * dY);
                if (t > 1.0)
                {
                    x1 = pnt2.X;
                    y1 = pnt2.Y;
                }
                else if (t > 0.0)
                {
                    x1 += dX * t;
                    y1 += dY * t;
                }
            }

            dX = pnt0.X - x1;
            dY = pnt0.Y - y1;
            return dX * dX + dY * dY;
        }
    }
}
