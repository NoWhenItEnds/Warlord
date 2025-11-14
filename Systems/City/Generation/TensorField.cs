using Godot;
using System;
using System.Collections.Generic;
using Warlord.City.Generation.Fields;

namespace Warlord.City.Generation
{
    /// <summary> A data structure representing a tensor field. </summary>
    /// <remarks> Used to for city generation. </remarks>
    public class TensorField
    {
        public readonly List<Field> Fields = new List<Field>();

        public readonly List<Vector2> Points = new List<Vector2>();


        private readonly Single DIAMETER;


        /// <summary> A data structure representing a tensor field. </summary>
        /// <param name="citySize"> The size of the city, and therefore the field, to generate. </param>
        /// <param name="origin"> The origin the field should be generated at. </param>
        /// <param name="diameter"></param>
        public TensorField(Vector2 citySize, Vector2 origin, Single diameter = 0.1f)
        {
            DIAMETER = diameter;

            Vector2 n = new Vector2(
                Mathf.Ceil(citySize.X / DIAMETER) + 1,
                Mathf.Ceil(citySize.Y / DIAMETER) + 1);

            for (Int32 x = 1; x < n.X - 1; x++)
            {
                for (Int32 y = 1; y < n.Y - 1; y++)
                {
                    Points.Add(new Vector2(origin.X + x * DIAMETER * 2, origin.Y + y * DIAMETER * 2));
                }
            }
        }


        public Tensor GetPoint(Vector2 position)
        {
            if (Fields.Count == 0) return new Tensor(1, new Single[] { 0, 0 });

            var tensorAcc = Tensor.Zero;

            foreach (var f in Fields) tensorAcc.Add(f.GetWeightedTensor(position));

            return tensorAcc;
        }


        public Vector2[] GetTensorLine(Vector2 point, Vector2 tensor)
        {
            var diff = tensor * DIAMETER;
            var start = point - diff;
            var end = point + diff;

            return [ start, end ];
        }


        public void AddRadial(Vector2 center, float size, float decay)
        {
            AddField(new Radial(center, size, decay));
        }

        public void AddGrid(Vector2 center, float size, float decay, float theta)
        {
            AddField(new Grid(center, size, decay, theta));
        }

        private void AddField(Field field)
        {
            Fields.Add(field);
        }
    }
}
