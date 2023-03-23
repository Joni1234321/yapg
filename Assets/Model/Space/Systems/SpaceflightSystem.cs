using Bserg.Model.Shared.Components;
using Bserg.Model.Shared.SystemGroups;
using Bserg.Model.Space.Components;
using Bserg.Model.Space.Systems.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

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
            state.RequireForUpdate<HohmannTransferMap>();
            
            populationLevels = state.GetComponentLookup<PopulationLevel>(true);
            populationProgresses = state.GetComponentLookup<PopulationProgress>();
            pools = state.GetComponentLookup<SpacecraftPool>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            int tick = SystemAPI.GetSingleton<GameTicks>().Ticks;
            HohmannTransferMap transfers = SystemAPI.GetSingleton<HohmannTransferMap>();
            
            state.Dependency.Complete();

  
            // Land
            EntityCommandBuffer ecb1 = new EntityCommandBuffer(Allocator.TempJob);
            new SpaceflightLandJob()
            {
                Ecb = ecb1,
                Ticks = tick,
            }.Run();
            ecb1.Playback(state.WorldUnmanaged.EntityManager);
            ecb1.Dispose();
            
            populationLevels.Update(ref state);
            populationProgresses.Update(ref state);
            pools.Update(ref state);

            // Process
            EntityCommandBuffer ecb2 = new EntityCommandBuffer(Allocator.TempJob);
            new SpaceflightProcessJob
            {
                Ecb = ecb2,
                Ticks = tick,
                PopulationLevels = populationLevels,
                PopulationProgresses = populationProgresses,
                Pools = pools,
                TransferMap = transfers.Map,
            }.Run();
            ecb2.Playback(state.WorldUnmanaged.EntityManager);
            ecb2.Dispose();

            // Take Off
            EntityCommandBuffer ecb3 = new EntityCommandBuffer(Allocator.TempJob);
            new SpaceflightTakeOffJob
            {
                Ecb = ecb3,
                Ticks = tick,
                TransferMap = transfers.Map,

            }.Run();
            ecb3.Playback(state.WorldUnmanaged.EntityManager);
            ecb3.Dispose();
        }



    }
}