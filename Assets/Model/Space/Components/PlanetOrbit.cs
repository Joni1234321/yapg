using Unity.Collections;
using Unity.Entities;

namespace Bserg.Model.Space.Components
{
    public struct PlanetOrbit : IComponentData
    {
        public Entity OrbitEntity;
    }

    public struct OrbitPeriod : IComponentData
    {
        public float TicksF;
    }

    public struct StandardGravitationalParameter : IComponentData
    {
        public const double GRAVITATIONAL_CONSTANT = 0.000_000_000_066_743;
        public double Value;
    }
    
    public struct HohmannTransferMap : IComponentData
    {
        public NativeHashMap<EntityPair, HohmannTransfer>.ReadOnly Map;
    }
    
}