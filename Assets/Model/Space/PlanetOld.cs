using Bserg.Model.Units;
using UnityEngine;

namespace Bserg.Model.Space
{
    public class PlanetOld
    {
        public readonly Vector3 StartingPosition;
        public readonly string Name;
        public readonly Color Color;
        public readonly float Size;
        public readonly Mass Mass;
        public readonly Length OrbitRadius;
        public readonly int OrbitObject;

        public float OuterRadius => (Size + 2f) / 2f;
        public readonly long CarryingCapacity;
        
        public PlanetOld(Vector3 startingPosition, string name, Color color, float size, Mass mass, Length orbitRadius, int orbitObject)
        {
            StartingPosition = startingPosition;
            Name = name;
            Color = color;
            Size = size;
            Mass = mass;
            OrbitRadius = orbitRadius;
            OrbitObject = orbitObject;
            

            // 0.5 = 12.5 Billion
            // 0.3 = 4 Billion
            CarryingCapacity = (long)(Size * Size * 50 * 1_000_000_000);
        }
    }
}