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


        /// <summary> How big each of the generated grid squares should be. </summary>
        private readonly Single DIAMETER;


        /// <summary> A data structure representing a tensor field. </summary>
        /// <param name="citySize"> The size of the city, and therefore the field, to generate. </param>
        /// <param name="origin"> The origin the field should be generated at. </param>
        /// <param name="diameter"> How big each of the generated grid squares should be. </param>
        public TensorField(Vector2 citySize, Vector2 origin, Single diameter = 1f)
        {
            DIAMETER = diameter;

            Vector2 n = new Vector2(
                Mathf.Ceil(citySize.X / DIAMETER) + 1,
                Mathf.Ceil(citySize.Y / DIAMETER) + 1);

            for (Int32 x = 1; x < n.X - 1; x++)
            {
                for (Int32 y = 1; y < n.Y - 1; y++)
                {
                    Points.Add(new Vector2(origin.X + x * DIAMETER, origin.Y + y * DIAMETER));
                }
            }
        }


        /// <summary> Get the tensor at the given position. </summary>
        /// <param name="position"> The position on the grid. </param>
        /// <returns> The </returns>
        public Tensor GetPoint(Vector2 position)
        {
            Tensor tensor = new Tensor(1, new Single[] { 0, 0 });

            if (Fields.Count > 0)
            {
                tensor = Tensor.Zero;
                foreach (Field field in Fields)
                {
                    tensor.Add(field.GetWeightedTensor(position));
                }
            }

            return tensor;
        }


        public Vector2[] GetTensorLine(Vector2 point, Vector2 tensor)
        {
            Vector2 diff = tensor * DIAMETER;
            Vector2 start = point - diff;
            Vector2 end = point + diff;

            return [ start, end ];
        }


        public void AddRadial(Vector2 center, float size, float decay) => Fields.Add(new Radial(center, size, decay));


        public void AddGrid(Vector2 center, float size, float decay, float theta) => Fields.Add(new Grid(center, size, decay, theta));
    }
}
