using Bserg.Model.Space.SpaceMovement;
using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Assertions;
using Spacecraft = Bserg.Model.Space.Components.Spacecraft;

namespace Bserg.Model.Space.Systems.Jobs
{
    /// <summary>
    /// Take off from planet
    /// ONLY RUN
    /// </summary>
    [BurstCompile]
    internal partial struct SpaceflightTakeOffJob : IJobEntity
    {
        public EntityCommandBuffer Ecb;
        [ReadOnly] public int Ticks;
        
        [ReadOnly] public NativeHashMap<EntityPair, HohmannTransfer>.ReadOnly TransferMap;

        
        public void Execute(Entity e, in Spacecraft.DepartureTick departure)
        {
            if (Hint.Likely(departure.Tick != Ticks))
                return;
            
            // Launch ship
            Ecb.RemoveComponent<Spacecraft.DepartureTick>(e);
        }
    }
}