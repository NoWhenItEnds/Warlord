using Godot;
using System;

namespace Warlord.City.Generation.Fields
{
    /// <summary> A generic tensor field type. </summary>
    public abstract class Field
    {
        /// <summary> The position at the center of the field on the grid. </summary>
        public Vector2 Center { get; private set; }

        // TODO - ???
        public Single Decay { get; private set; }

        /// <summary> The radius of the tensor field. </summary>
        public Single Size { get; private set; }

        protected Field(Vector2 center, Single size, Single decay)
        {
            Center = center;
            Size = size;
            Decay = decay;
        }

        public Tensor GetWeightedTensor(Vector2 point)
        {
            Tensor tensor = GetTensor(point);
            tensor.Radius *= GetTensorWeight(point);
            tensor.IsThetaDirty = true;

            return tensor;
        }

        private Single GetTensorWeight(Vector2 point)
        {
            Single normDistanceToCenter = (point - Center).Length() / Size;

            if (Decay == 0 && normDistanceToCenter >= 1) return 0;

            return Mathf.Pow(Mathf.Max(0, 1 - normDistanceToCenter), Decay);
        }

        protected abstract Tensor GetTensor(Vector2 point);
    }


    public class Grid : Field
    {
        public Single Theta { get; private set; }

        public Grid(Vector2 center, Single size, Single decay, Single theta) : base(center, size, decay)
        {
            Theta = theta;
        }

        protected override Tensor GetTensor(Vector2 point) => new Tensor(1, [Mathf.Cos(2 * Theta), Mathf.Sin(2 * Theta)]);
    }


    public class Radial : Field
    {
        public Radial(Vector2 center, Single size, Single decay) : base(center, size, decay) {}


        protected override Tensor GetTensor(Vector2 point)
        {
            Vector2 t = point - Center;
            Single t1 = Mathf.Pow(t.Y, 2) - Mathf.Pow(t.X, 2);
            Single t2 = -2 * t.X * t.Y;

            return new Tensor(1, [ t1, t2 ]);
        }
    }
}
