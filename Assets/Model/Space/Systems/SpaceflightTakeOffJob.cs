using Bserg.Model.Space;
using Bserg.Model.Space.Components;
using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Assertions;

namespace Bserg.Model.Space.Systems
{
    /// <summary>
    /// Take off from planet
    /// ONLY RUN
    /// </summary>
    [BurstCompile]
    internal partial struct SpaceflightTakeOffJob : IJobEntity
    {
        public EntityCommandBuffer Ecb;
        [ReadOnly] public int GameTick;
        
        public void Execute(Entity e, in Spacecraft.DepartureTick departure)
        {
            if (Hint.Likely(departure.Tick != GameTick))
                return;
            
            // Launch ship
            Ecb.RemoveComponent<Spacecraft.DepartureTick>(e);
            int arrivalTick = GameTick + 50;
            Ecb.AddComponent(e, new Spacecraft.ArrivalTick { Tick = arrivalTick });

            Assert.IsTrue(arrivalTick > GameTick);
        }
    }
}