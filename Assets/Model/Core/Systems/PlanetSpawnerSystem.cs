using Bserg.Model.Population.Components;
using Bserg.Model.Shared.Components;
using Bserg.Model.Space.Components;
using Bserg.Model.Units;
using Bserg.Model.Utilities;
using Model.Utilities;

using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Bserg.Model.Core.Systems
{
    /// <summary>
    /// Converts all PlanetPrefabData into Planets
    /// </summary>
    internal partial struct PlanetSpawnerSystem : ISystem
    {
        private EntityQuery prefabQuery;
        public void OnCreate(ref SystemState state)
        {
            prefabQuery = state.GetEntityQuery(typeof(PlanetPrefabData));
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            if (Hint.Likely(prefabQuery.CalculateEntityCount() == 0))
                return;

            ConvertPrefabData(ref state);
        }
        
        void ConvertPrefabData (ref SystemState state)
        {

            var bufferSystem = state.World.GetExistingSystemManaged<BeginSimulationEntityCommandBufferSystem>();
            EntityCommandBuffer ecb = bufferSystem.CreateCommandBuffer();

            var deps = new ConvertJob
            {
                ParallelWriter = ecb.AsParallelWriter(),
            }.ScheduleParallel(state.Dependency);

            state.Dependency = JobHandle.CombineDependencies(state.Dependency, deps);
            deps.Complete();
            Debug.Log($"Converted {prefabQuery.CalculateEntityCount()}");
        }
    }

    
    /// <summary>
    /// Converts PlanetPrefabData to PlanetEntities
    /// </summary>
    [BurstCompile]
    internal partial struct ConvertJob : IJobEntity
    {

        public EntityCommandBuffer.ParallelWriter ParallelWriter;

        public void Execute([EntityIndexInQuery] int chunkIndex, Entity e, in PlanetPrefabData data)
        {
            // TODO: Spawn GameObject
            Entity planet = ParallelWriter.CreateEntity(chunkIndex);

            #if UNITY_EDITOR
            ParallelWriter.SetName(chunkIndex, planet, data.Name);
            #endif
            
            // Tag
            ParallelWriter.AddComponent(chunkIndex, planet, new PlanetTag());
            
            // Data
            ParallelWriter.AddComponent(chunkIndex, planet, new PlanetData
            {
                Color = data.Color,
                Size = data.Size,
                Mass = new Mass(data.WeightEarthMass, Mass.UnitType.EarthMass),
                OrbitRadius = new Length(data.RadiusAU, Length.UnitType.AstronomicalUnits),
            });
            ParallelWriter.AddComponent(chunkIndex, planet, new PlanetName { Name = data.Name });
            
            // Orbit
            ParallelWriter.AddComponent(chunkIndex, planet, new PlanetOrbit { OrbitID = data.OrbitObject });

            // Population
            if (data.Population > 0)
                ParallelWriter.AddComponent(chunkIndex, planet, new PlanetActiveTag());
            ParallelWriter.AddComponent(chunkIndex, planet, new PopulationLevel { Level = (int)data.Population });
            ParallelWriter.AddComponent(chunkIndex, planet, new PopulationProgress { Progress = data.Population - (int)data.Population });
            
            // Growth System
            ParallelWriter.AddComponent(chunkIndex, planet, new PopulationGrowth { BirthRate = 0.02f, DeathRate = 0.005f } );
            
            // Destroy old
            ParallelWriter.DestroyEntity(chunkIndex, e);
        }
    }

}