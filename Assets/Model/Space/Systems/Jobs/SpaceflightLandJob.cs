using Bserg.Model.Space.Components;
using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Entities;

namespace Bserg.Model.Space.Systems.Jobs
{
    /// <summary>
    /// Land on planet
    /// </summary>
    [BurstCompile]
    [WithAll(typeof(Spacecraft.FlyingTag))]
    internal partial struct SpaceflightLandJob : IJobEntity
    {
        public EntityCommandBuffer Ecb;
        [ReadOnly] public int Ticks;

        public void Execute(Entity e, in Spacecraft.ArrivalTick arrival)
        {
            if (Hint.Likely((int)arrival.TickF != Ticks))
                return;

            // Land ship
            Ecb.RemoveComponent<Spacecraft.DepartureTick>(e);
            Ecb.RemoveComponent<Spacecraft.ArrivalTick>(e);
            Ecb.RemoveComponent<Spacecraft.FlyingTag>(e);

            // Process
            Ecb.AddComponent<Spacecraft.ProcessingTag>(e);
        }
    }
}