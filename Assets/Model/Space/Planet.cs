using Bserg.Model.Units;
using UnityEngine;

namespace Bserg.Model.Space
{
    public class Planet
    {
        public Vector3 Position;
        public readonly float Size;
        public readonly Mass Mass;
        public readonly Length OrbitRadius;

        public float OuterRadius => (Size + 2f) / 2f;
        public readonly long CarryingCapacity;
        
        public Planet(Vector3 position, float size, Mass mass, Length orbitRadius)
        {
            Position = position;
            Size = size;
            Mass = mass;
            OrbitRadius = orbitRadius;
            

            // 0.5 = 12.5 Billion
            // 0.3 = 4 Billion
            CarryingCapacity = (long)(Size * Size * 50 * 1_000_000_000);
        }
    }


    public class BuildOrder
    {
        public readonly string Name; 
        public int Progress;

        public BuildOrder(string name)
        {
            Name = name;
            Progress = 0;
        }
    }
}