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
    internal partial struct SpaceflightLandJob : IJobEntity
    {
        public EntityCommandBuffer Ecb;
        [ReadOnly] public int GameTick;

        public void Execute(Entity e, in Spacecraft.ArrivalTick arrival)
        {
            if (Hint.Likely(arrival.Tick != GameTick))
                return;

            // Land ship
            Ecb.RemoveComponent<Spacecraft.ArrivalTick>(e);
            Ecb.AddComponent(e, new Spacecraft.ProcessingTag());
        }
    }
}