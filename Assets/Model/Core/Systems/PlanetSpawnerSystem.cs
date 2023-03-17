using Bserg.Model.Population.Components;
using Bserg.Model.Shared.Components;
using Bserg.Model.Space.Components;
using Bserg.Model.Units;

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
            prefabQuery = state.GetEntityQuery(typeof(Planet.PrefabData));
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (Hint.Likely(prefabQuery.CalculateEntityCount() == 0))
                return;

            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

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

        public void Execute([ChunkIndexInQuery] int i, Entity e, in Planet.PrefabData data)
        {
            // TODO: Spawn GameObject
            // Entity e = ParallelWriter.CreateEntity(i);

            #if UNITY_EDITOR
            ParallelWriter.SetName(i, e, data.Name);
            #endif
            
            // Tag
            ParallelWriter.AddComponent(i, e, new Planet.Tag());
            
            // Data
            ParallelWriter.AddComponent(i, e, new Planet.Data
            {
                Color = data.Color,
                Size = data.Size,
                Mass = new Mass(data.WeightEarthMass, Mass.UnitType.EarthMass),
                OrbitRadius = new Length(data.RadiusAU, Length.UnitType.AstronomicalUnits),
            });
            ParallelWriter.AddComponent(i, e, new Planet.Name { Text = data.Name });
            
            // Orbit
            ParallelWriter.AddComponent(i, e, new PlanetOrbit { OrbitID = data.OrbitObject });

            // Population
            if (data.Population > 0)
                ParallelWriter.AddComponent(i, e, new PlanetActiveTag());
            ParallelWriter.AddComponent(i, e, new PopulationLevel { Level = (int)data.Population });
            ParallelWriter.AddComponent(i, e, new PopulationProgress { Progress = data.Population - (int)data.Population });
            
            // Levels
            ParallelWriter.AddComponent(i, e, new HousingLevel { Level = 50 });
            ParallelWriter.AddComponent(i, e, new FoodLevel { Level = 0 });
            ParallelWriter.AddComponent(i, e, new HousingLevel { Level = 0 });
            
            // Growth System
            ParallelWriter.AddComponent(i, e, new PopulationGrowth { BirthRate = 0.02f, DeathRate = 0.005f } );
            
            // Spacecraft System
            ParallelWriter.AddComponent(i, e, new SpacecraftPool { Available = data.Population > 30 ? 4000 : 0});
            
            // Destroy old
            ParallelWriter.RemoveComponent<Planet.PrefabData>(i, e);
        }
    }

}