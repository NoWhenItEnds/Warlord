using Godot;
using System;

namespace Warlord.Utilities.Extensions
{
    /// <summary> Helpful mathematic extensions. </summary>
    public static class MathExtensions
    {
        /// <summary> Min value for float calculations </summary>
        public static Single FLOAT_EPSILON => 0.00001f;

        /// <summary> Safe min value for float calculations that includes a buffer area </summary>
        public static Single SAFE_FLOAT_EPSILON => 0.001f;


        /// <summary> Calculates the angle between 2 vectors in radians </summary>
        /// <param name="from"> The first vector. </param>
        /// <param name="to"> The second vector. </param>
        /// <returns> The angle between 2 vectors (in radians). </returns>
        public static Single AngleBetween(Vector3 from, Vector3 to)
        {
            Vector3 fromN = Normalize(from);
            Vector3 toN = Normalize(to);

            return (Single)Math.Acos(Mathf.Clamp(Dot(fromN, toN), -1F, 1F));
        }


        /// <summary> Calculates how closely two vectors align in terms of the directions they point. </summary>
        /// <param name="from"> The first vector. </param>
        /// <param name="to"> The second vector. </param>
        /// <returns> Dot (scalar) Product of two vectors. </returns>
        private static Single Dot(Vector3 from, Vector3 to)
        {
            Single dot = from.X * to.X + from.Y * to.Y + from.Z * to.Z;
            return dot;
        }


        /// <summary> Keep the direction of a vector, but set its length to 1.0. </summary>
        /// <param name="from"> The vector to normalise. </param>
        /// <returns> A vector with its length set to 1.0. </returns>
        private static Vector3 Normalize(Vector3 from)
        {
            Single mag = Magnitude(from);

            if (mag > FLOAT_EPSILON)
            {
                return from / mag;
            }
            else
            {
                return Vector3.Zero;
            }
        }


        /// <summary> Calculates the length of a given vector. </summary>
        /// <param name="vector"> The vector to compute. </param>
        /// <returns> Length of the vector. </returns>
        private static Single Magnitude(Vector3 vector)
        {
            Double vectorSqrt = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
            return (Single)vectorSqrt;
        }
    }
}
