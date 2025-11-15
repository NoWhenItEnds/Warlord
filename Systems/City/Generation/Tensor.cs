using Godot;
using System;

namespace Warlord.City.Generation
{
    /// <summary> A data class representing a position within the tensor field. </summary>
    public class Tensor
    {
        /// <summary> The theta calculated from the matrix. </summary>
        public Single Theta
        {
            get
            {
                if (IsThetaDirty)
                {
                    _theta = CalculateTheta();
                    IsThetaDirty = false;
                }

                return _theta;
            }
            init => _theta = value;
        }
        private Single _theta;


        /// <summary> The tensor's major axis as a vector. </summary>
        public Vector2 Major
        {
            get
            {
                Vector2 vector = Vector2.Zero;
                if (Radius != 0)
                {
                    vector = new Vector2(Mathf.Cos(Theta), Mathf.Sin(Theta));
                }
                return vector;
            }
        }


        /// <summary> The tensor's minor axis as a vector. </summary>
        public Vector2 Minor
        {
            get
            {
                Vector2 vector = Vector2.Zero;
                if (Radius != 0)
                {
                    Single angle = Theta + Mathf.Pi * 0.5f;
                    vector = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                }
                return vector;
            }
        }

        /// <summary> The tensor's scalar, in this case the radius. </summary>
        public Single Radius;

        /// <summary> Whether the theta needs to be recalculated. </summary>
        public Boolean IsThetaDirty;


        /// <summary> The tensor's internal matrix. </summary>
        private readonly Single[] _matrix;


        public Tensor(Single radius, Single[] matrix)
        {
            Radius = radius;
            _matrix = matrix;
            IsThetaDirty = false;
            Theta = CalculateTheta();
        }


        /// <summary> A tensor representing an empty or null value. </summary>
        public static Tensor Zero => new Tensor(0, new Single[] { 0, 0 });


        /// <summary> Adds another tensor to this one, modifying it. </summary>
        /// <param name="tensor"> The tensor to add. </param>
        public void Add(Tensor tensor)
        {
            for (Int32 i = 0; i < _matrix.Length; i++)
            {
                _matrix[i] = _matrix[i] * Radius + tensor._matrix[i] * tensor.Radius;
            }

            Radius = 2;
            IsThetaDirty = true;
        }


        /// <summary> Recalculate the theta based upon the tensor's matrix and radius. </summary>
        /// <returns> The n</returns>
        private Single CalculateTheta()
        {
            Single theta = 0.0f;
            if (Radius != 0)
            {
                theta = Mathf.Atan2(_matrix[1] / Radius, _matrix[0] / Radius) * 0.5f;
            }
            return theta;
        }
    }
}
