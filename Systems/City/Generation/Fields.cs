using Godot;
using System;

namespace Warlord.City.Generation.Fields
{
    public abstract class Field
    {
        public Vector2 center;
        public Single decay;
        public Single size;

        protected Field(Vector2 center, Single size, Single decay)
        {
            this.center = center;
            this.size = size;
            this.decay = decay;
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
            var normDistanceToCenter = (point - center).Length() / size;

            if (decay == 0 && normDistanceToCenter >= 1) return 0;

            return Mathf.Pow(Mathf.Max(0, 1 - normDistanceToCenter), decay);
        }

        protected abstract Tensor GetTensor(Vector2 point);
    }


    public class Grid : Field
    {
        public float theta;

        public Grid(Vector2 center, float size, float decay, float theta) : base(center, size, decay)
        {
            this.theta = theta;
        }

        protected override Tensor GetTensor(Vector2 point)
        {
            return new Tensor(1, [ Mathf.Cos(2 * theta), Mathf.Sin(2 * theta) ]);
        }
    }


    public class Radial : Field
    {
        public Radial(Vector2 center, float size, float decay) : base(center, size, decay) {}


        protected override Tensor GetTensor(Vector2 point)
        {
            Vector2 t = point - center;
            Single t1 = Mathf.Pow(t.Y, 2) - Mathf.Pow(t.X, 2);
            Single t2 = -2 * t.X * t.Y;

            return new Tensor(1, [ t1, t2 ]);
        }
    }
}
