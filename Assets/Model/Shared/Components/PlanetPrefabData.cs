using Bserg.Model.Units;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bserg.Model.Shared.Components
{
    /// <summary>
    /// Copy of scriptable object
    /// </summary>
    public struct PlanetPrefabData : IComponentData
    {
        public FixedString32Bytes Name;
        public long Population;

        public float4 Color;

        public float Size;
        public double WeightEarthMass;
        public double RadiusAU;
        public int OrbitObject;
    }
    
    public struct PlanetTag : IComponentData
    {
        
    }
    public struct PlanetData : IComponentData
    {
        public float4 Color;
        public float Size;
        public Mass Mass;
        public Length OrbitRadius;
    }
    
    public struct PlanetName : IComponentData
    {
        public FixedString32Bytes Name;
    }
    
}