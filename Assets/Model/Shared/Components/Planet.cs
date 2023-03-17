using Bserg.Model.Units;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bserg.Model.Shared.Components
{

    public struct Planet
    {
        /// <summary>
        /// Copy of scriptable object
        /// </summary>
        public struct PrefabData : IComponentData
        {
            public FixedString32Bytes Name;
            public long Population;

            public float4 Color;

            public float Size;
            public double WeightEarthMass;
            public double RadiusAU;
            public int OrbitObject;
        }
        
        public struct PrefabEntity : IComponentData
        {
            public Entity Prefab;
        }

        public struct Tag : IComponentData
        {

        }

        public struct Data : IComponentData
        {
            public float4 Color;
            public float Size;
            public Mass Mass;
            public Length OrbitRadius;
        }

        public struct Name : IComponentData
        {
            public FixedString32Bytes Text;
        }
    }
}