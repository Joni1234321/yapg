using Bserg.Model.Population.Components;
using Bserg.Model.Shared.Components;
using Bserg.Model.Space.Components;
using Bserg.Model.Units;

using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Planet = Bserg.Model.Shared.Components.Planet;
using StandardGravitationalParameter = Bserg.Model.Space.Components.StandardGravitationalParameter;

namespace Bserg.Model.Core.Systems
{
    //[UpdateInGroup(typeof())]
    /// <summary>
    /// Converts all PlanetPrefabData into Planets
    /// </summary>
    [RequireMatchingQueriesForUpdate]
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
            int n = prefabQuery.CalculateEntityCount();
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            
            NativeArray<Entity> entities = new NativeArray<Entity>(n, Allocator.TempJob);
            var deps = new ConvertJob
            {
                Entities = entities,
                ParallelWriter = ecb.AsParallelWriter(),
            }.ScheduleParallel(state.Dependency);

            state.Dependency = JobHandle.CombineDependencies(state.Dependency, deps);
            deps.Complete();
            entities.Dispose();
            Debug.Log($"Converted {prefabQuery.CalculateEntityCount()}");

            while (Hint.Unlikely(prefabQuery.CalculateEntityCount() == 0))
            {
                // Convert
                // Add children
            }
        }

    }

    
    /// <summary>
    /// Converts PlanetPrefabData to PlanetEntities
    /// </summary>
    [BurstCompile]
    internal partial struct ConvertJob : IJobEntity
    {
        public NativeArray<Entity> Entities;
        public EntityCommandBuffer.ParallelWriter ParallelWriter;

        public void Execute([EntityIndexInQuery] int i, Entity e, in Planet.PrefabData data)
        {
            // TODO: Spawn GameObject
            // Entity e = ParallelWriter.CreateEntity(i);

            Entities[i] = e;
            
            #if UNITY_EDITOR
            ParallelWriter.SetName(i, e, data.Name);
            #endif
            
            // Tag
            ParallelWriter.AddComponent(i, e, new Planet.Tag());
            
            // Data
            Mass mass = new Mass(data.WeightEarthMass, Mass.UnitType.EarthMass);
            Length radius = new Length(data.RadiusAU, Length.UnitType.AstronomicalUnits);
            ParallelWriter.AddComponent(i, e, new Planet.Data
            {
                Color = data.Color,
                Size = data.Size,
                Mass = mass,
                OrbitRadius = radius,
            });
            ParallelWriter.AddComponent(i, e, new Planet.Name { Text = data.Name });
            
            // Population
            if (data.Population > 0)
                ParallelWriter.AddComponent(i, e, new Planet.ActiveTag());
            ParallelWriter.AddComponent(i, e, new PopulationLevel { Level = (int)data.Population });
            ParallelWriter.AddComponent(i, e, new PopulationProgress { Progress = data.Population - (int)data.Population });
            
            // Levels
            ParallelWriter.AddComponent(i, e, new HousingLevel { Level = 50 });
            ParallelWriter.AddComponent(i, e, new FoodLevel { Level = 0 });
            ParallelWriter.AddComponent(i, e, new HousingLevel { Level = 0 });
            
            // Growth System
            ParallelWriter.AddComponent(i, e, new PopulationGrowth { BirthRate = 0.02f, DeathRate = 0.005f } );
            
            // OrbitSystem
            ParallelWriter.AddComponent(i, e, new StandardGravitationalParameter 
                { Value = StandardGravitationalParameter.GRAVITATIONAL_CONSTANT * mass.To(Mass.UnitType.KiloGrams) });

            
            // Spacecraft System
            ParallelWriter.AddComponent(i, e, new SpacecraftPool { Available = data.Population > 30 ? 4000 : 0});
            
            // Destroy old
            ParallelWriter.RemoveComponent<Planet.PrefabData>(i, e);
        }
    }



}