using Bserg.Model.Shared.Components;
using Bserg.Model.Shared.SystemGroups;
using Bserg.Model.Space.Components;
using Bserg.Model.Space.Systems.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Bserg.Model.Space.Systems
{
    /// <summary>
    /// Fly all spaceships
    /// if they are at their destination, send them to pool or send them on new journey
    /// </summary>
    [UpdateInGroup(typeof(TickSystemGroup))]
    internal partial struct SpaceflightSystem : ISystem
    {
        private ComponentLookup<PopulationProgress> populationProgresses;
        private ComponentLookup<PopulationLevel> populationLevels;
        private ComponentLookup<SpacecraftPool> pools;


        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameTicks>();
            populationLevels = state.GetComponentLookup<PopulationLevel>(true);
            populationProgresses = state.GetComponentLookup<PopulationProgress>();
            pools = state.GetComponentLookup<SpacecraftPool>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            int tick = SystemAPI.GetSingleton<GameTicks>().Ticks;
            state.Dependency.Complete();

            populationLevels.Update(ref state);
            populationProgresses.Update(ref state);
            pools.Update(ref state);
            
            // Land
            EntityCommandBuffer ecb1 = new EntityCommandBuffer(Allocator.TempJob);
            new SpaceflightProcessJob
            {
                Ecb = ecb1,
                GameTick = tick,
                PopulationLevels = populationLevels,
                PopulationProgresses = populationProgresses,
                Pools = pools,
            }.Run();
            ecb1.Playback(state.WorldUnmanaged.EntityManager);
            ecb1.Dispose();
            
            // Process
            EntityCommandBuffer ecb2 = new EntityCommandBuffer(Allocator.TempJob);
            new SpaceflightLandJob()
            {
                Ecb = ecb2,
                GameTick = tick,
            }.Run();
            ecb2.Playback(state.WorldUnmanaged.EntityManager);
            ecb2.Dispose();

            // Take Off
            EntityCommandBuffer ecb3 = new EntityCommandBuffer(Allocator.TempJob);
            new SpaceflightTakeOffJob
            {
                Ecb = ecb3,
                GameTick = tick,
            }.Run();
            ecb3.Playback(state.WorldUnmanaged.EntityManager);
            ecb3.Dispose();
        }



    }
}