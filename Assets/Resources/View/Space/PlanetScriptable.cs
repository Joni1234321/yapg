using UnityEngine;

namespace Bserg.View.Space
{
    [System.Serializable]
    public struct PlanetScriptable
    {
        public  string Name;
        public long Population;

        public Material material;
        public Color Color;

        public float Size;
        public double WeightEarthMass;
        public double RadiusAU;
    }
}
