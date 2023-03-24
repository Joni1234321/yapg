using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Spacecraft = Bserg.Model.Space.Components.Spacecraft;

namespace Bserg.Model.Space.Systems.Jobs
{
    /// <summary>
    /// Take off from planet
    /// ONLY RUN
    /// </summary>
    [BurstCompile]
    [WithNone(typeof(Spacecraft.TravelingTag), typeof(Spacecraft.ProcessingTag))]
    internal partial struct SpaceflightTakeOffJob : IJobEntity
    {
        public EntityCommandBuffer Ecb;
        [ReadOnly] public int Ticks;
        
        [ReadOnly] public NativeHashMap<EntityPair, HohmannTransfer>.ReadOnly TransferMap;

        
        public void Execute(Entity e, in Spacecraft.DepartureTick departure)
        {
            if (Hint.Likely((int)departure.TickF != Ticks))
                return;
            
            // Launch ship
            Ecb.AddComponent<Spacecraft.TravelingTag>(e);
        }
    }
}