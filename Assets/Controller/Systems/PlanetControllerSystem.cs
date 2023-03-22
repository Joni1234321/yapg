using Bserg.Controller.Components;
using Bserg.Controller.Core;
using Bserg.Controller.World;
using Bserg.Model.Shared.SystemGroups;
using Bserg.Model.Space.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Bserg.Controller.Systems
{
    [UpdateInGroup(typeof(DrawSystemGroup))] 
    internal partial struct PlanetControllerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameTicksF>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float ticksF = SystemAPI.GetSingleton<GameTicksF>().TicksF;

            
            var planetDeps = new PlanetRenderJob
            {
                GameTicksF = ticksF,
            }.ScheduleParallel(state.Dependency);
            var orbitDeps = new OrbitRenderJob
            {
                GameTicksF = ticksF,
            }.ScheduleParallel(planetDeps); // For some reason it cant reason that withnone and with all that are opposite doesnt slice
            
            state.Dependency = JobHandle.CombineDependencies(planetDeps, orbitDeps);
            state.Dependency.Complete();
            
        }
    }

    [BurstCompile]
    internal partial struct PlanetRenderJob : IJobEntity
    {
        [ReadOnly] public float GameTicksF;
        public void Execute(ref LocalTransform localTransform, in OrbitRadius orbitRadius, in OrbitPeriod orbitPeriod)
        {
            localTransform.Position = 
                PlanetRenderer.GetLocalPlanetPositionAtTickF(orbitRadius.RadiusAU, orbitPeriod.TicksF, GameTicksF);

        } 
    }
    
    [BurstCompile]
    [WithAll(typeof(OrbitRotateTag))]   
    internal partial struct OrbitRenderJob : IJobEntity
    {
        [ReadOnly] public float GameTicksF;
        public void Execute(ref LocalTransform localTransform, in OrbitPeriod orbitPeriod)
        {
            float angle = PlanetRenderer.GetPlanetAngleAtTicksF(orbitPeriod.TicksF, GameTicksF);
            localTransform.Rotation = Quaternion.Euler(0, 0, 90 + math.degrees(angle));
        } 
    }
}