using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace JoltMonogameTest
{
    public static class Extensions
    {
        public static Vector3 ToXnaVector(this System.Numerics.Vector3 vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

        public static float RandFloat(this Random random, float min, float max)
        {
            // Generate a random float between 0.0 (inclusive) and 1.0 (exclusive)
            float randomFloat = (float)random.NextDouble();

            // Scale the random float to the desired range [min, max]
            return min + (randomFloat * (max - min));
        }
    }
}
